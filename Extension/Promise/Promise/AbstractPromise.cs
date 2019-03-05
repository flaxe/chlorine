namespace Chlorine.Internal
{
	public abstract class AbstractPromise : IPoolable
	{
		protected enum PromiseStatus
		{
			Pending,
			Resolved,
			Rejected
		}

		private WeakReferenceList<Future> _futures;

		private PromiseStatus _status = PromiseStatus.Pending;
		private Error _reason;

		protected AbstractPromise()
		{
		}

		public bool IsResolved => _status == PromiseStatus.Resolved;
		public bool IsRejected => _status == PromiseStatus.Rejected;

		protected PromiseStatus Status
		{
			get => _status;
			set => _status = value;
		}

		protected Error Reason => _reason;

		public virtual void Reset()
		{
			RevokeAll();
			_status = PromiseStatus.Pending;
			_reason = default;
		}

		public bool TryGetReason(out Error reason)
		{
			if (_status == PromiseStatus.Rejected)
			{
				reason = _reason;
				return true;
			}
			reason = default;
			return false;
		}

		public void Fulfill(Future future)
		{
			switch (_status)
			{
				case PromiseStatus.Pending:
					if (_futures == null)
					{
						_futures = new WeakReferenceList<Future>();
					}
					_futures.Add(future);
					break;
				case PromiseStatus.Resolved:
					future.Resolve();
					break;
				case PromiseStatus.Rejected:
					future.Reject(_reason);
					break;
			}
		}

		public void Revoke(Future future)
		{
			_futures?.Remove(future);
		}

		public virtual void RevokeAll()
		{
			_futures?.Clear();
		}

		public void Reject(Error reason)
		{
			if (_status == PromiseStatus.Pending)
			{
				_status = PromiseStatus.Rejected;
				_reason = reason;
				HandleReject();
				RevokeAll();
			}
		}

		protected virtual void HandleResolve()
		{
			if (_futures != null)
			{
				foreach (Future future in _futures)
				{
					future.Resolve();
				}
			}
		}

		private void HandleReject()
		{
			if (_futures != null && _futures.Count > 0)
			{
				foreach (Future future in _futures)
				{
					future.Reject(_reason);
				}
			}
		}
	}
}