using Chlorine.Collections;
using Chlorine.Exceptions;

namespace Chlorine.Internal
{
	public abstract class AbstractFalsePromise : IPromise, IPoolable
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

		protected AbstractFalsePromise()
		{
		}

		public bool IsResolved => _status == PromiseStatus.Resolved;
		public bool IsRejected => _status == PromiseStatus.Rejected;

		protected PromiseStatus Status
		{
			get => _status;
			set => _status = value;
		}

		public Error Reason
		{
			get
			{
				if (_status != PromiseStatus.Rejected)
				{
					throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
							"Invalid operation. Promise was not rejected.");
				}
				return _reason;
			}
		}

		public virtual void Reset()
		{
			Clear();
			_status = PromiseStatus.Pending;
			_reason = default;
		}

		public virtual void Clear()
		{
			_futures?.Clear();
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

		public void Reject(Error reason)
		{
			if (_status != PromiseStatus.Pending)
			{
				throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
						"Invalid operation. Promise already resolved or rejected.");
			}
			_status = PromiseStatus.Rejected;
			_reason = reason;
			HandleReject();
			Clear();
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