using Chlorine.Collections;
using Chlorine.Exceptions;

namespace Chlorine.Futures.Internal
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

		public bool IsPending => _status == PromiseStatus.Pending;
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
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise was not rejected.");
				}
				return _reason;
			}
		}

		public virtual void Reset()
		{
			HandleClear();
			_status = PromiseStatus.Pending;
			_reason = default;
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
				throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
						"Promise already resolved or rejected.");
			}
			_status = PromiseStatus.Rejected;
			_reason = reason;
			HandleReject();
			HandleClear();
		}

		protected virtual void HandleClear()
		{
			_futures?.Clear();
		}

		protected virtual void HandleResolve()
		{
			if (_futures != null && _futures.Count > 0)
			{
				foreach (Future future in _futures)
				{
					future.Resolve();
				}
			}
		}

		protected virtual void HandleReject()
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