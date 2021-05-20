using Carbone.Collections;
using Carbone.Exceptions;

namespace Carbone.Futures.Internal
{
	public abstract class AbstractFalsePromise : IPromise, IPoolable
	{
		protected enum PromiseStatus
		{
			Empty,
			Pending,
			Resolved,
			Rejected
		}

		private WeakReferenceList<Future>? _futures;

		private PromiseStatus _status;
		private Error _reason;

		protected AbstractFalsePromise()
		{
			_status = PromiseStatus.Empty;
		}

		public bool IsPending
		{
			get
			{
				if (_status == PromiseStatus.Empty)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise is empty.");
				}
				return _status == PromiseStatus.Pending;
			}
		}

		public bool IsResolved
		{
			get
			{
				if (_status == PromiseStatus.Empty)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise is empty.");
				}
				return _status == PromiseStatus.Resolved;
			}
		}

		public bool IsRejected
		{
			get
			{
				if (_status == PromiseStatus.Empty)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise is empty.");
				}
				return _status == PromiseStatus.Rejected;
			}
		}

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
			_status = PromiseStatus.Empty;
			_reason = default;
		}

		public void Init()
		{
			if (_status != PromiseStatus.Empty)
			{
				throw new ReuseException(this);
			}
			_status = PromiseStatus.Pending;
		}

		public void Fulfill(Future future)
		{
			switch (_status)
			{
				case PromiseStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise is empty.");
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
			switch (_status)
			{
				case PromiseStatus.Resolved:
				case PromiseStatus.Rejected:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise already resolved or rejected.");
				default:
					_status = PromiseStatus.Rejected;
					_reason = reason;
					HandleReject();
					HandleClear();
					break;
			}
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