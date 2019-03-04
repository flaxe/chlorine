namespace Chlorine.Internal
{
	public class AbstractFuture : IPoolable
	{
		protected enum FutureStatus
		{
			Pending,
			Resolved,
			Rejected
		}

		private FutureStatus _status = FutureStatus.Pending;

		private FutureResolved _resolved;
		private FutureRejected _rejected;

		private Error _reason;

		protected AbstractFuture()
		{
		}

		protected FutureStatus Status
		{
			get => _status;
			set => _status = value;
		}

		public bool IsResolved => _status == FutureStatus.Resolved;
		public bool IsRejected => _status == FutureStatus.Rejected;

		public Error Reason => _reason;

		public virtual void Reset()
		{
			Clear();
			_status = FutureStatus.Pending;
			_reason = default;
		}

		public virtual void Clear()
		{
			_resolved = null;
			_rejected = null;
		}

		public void Then(FutureResolved resolved, FutureRejected rejected)
		{
			switch (_status)
			{
				case FutureStatus.Pending:
					_resolved += resolved;
					break;
				case FutureStatus.Resolved:
					resolved.Invoke();
					break;
			}
			Catch(rejected);
		}

		public void Catch(FutureRejected rejected)
		{
			switch (_status)
			{
				case FutureStatus.Pending:
					_rejected += rejected;
					break;
				case FutureStatus.Rejected:
					rejected.Invoke(_reason);
					break;
			}
		}

		public void Reject(Error reason)
		{
			if (_status == FutureStatus.Pending)
			{
				_status = FutureStatus.Rejected;
				_reason = reason;
				HandleReject();
				Clear();
			}
		}

		protected virtual void HandleResolve()
		{
			_resolved?.Invoke();
		}

		private void HandleReject()
		{
			_rejected?.Invoke(_reason);
		}
	}
}