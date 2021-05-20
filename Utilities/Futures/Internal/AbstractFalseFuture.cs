using System;
using Carbone.Exceptions;

namespace Carbone.Futures.Internal
{
	public abstract class AbstractFalseFuture : IFuture, IPoolable
	{
		protected enum FutureStatus
		{
			Empty,
			Pending,
			Resolved,
			Rejected
		}

		private FutureStatus _status;

		private FutureResolved? _resolved;
		private FutureRejected? _rejected;
		private IFutureHandler? _finalizer;

		private Error _reason;

		protected AbstractFalseFuture()
		{
			_status = FutureStatus.Empty;
		}

		protected AbstractFalseFuture(Error reason)
		{
			_status = FutureStatus.Rejected;
			_reason = reason;
		}

		public bool IsPending
		{
			get
			{
				if (_status == FutureStatus.Empty)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				}
				return _status == FutureStatus.Pending;
			}
		}

		public bool IsResolved
		{
			get
			{
				if (_status == FutureStatus.Empty)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				}
				return _status == FutureStatus.Resolved;
			}
		}

		public bool IsRejected
		{
			get
			{
				if (_status == FutureStatus.Empty)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				}
				return _status == FutureStatus.Rejected;
			}
		}

		protected FutureStatus Status
		{
			get => _status;
			set => _status = value;
		}

		public Error Reason
		{
			get
			{
				if (_status != FutureStatus.Rejected)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future was not rejected.");
				}
				return _reason;
			}
		}

		public virtual void Reset()
		{
			HandleClear();
			_status = FutureStatus.Empty;
			_reason = default;
		}

		public IFuture Then(FuturePromised promised)
		{
			switch (_status)
			{
				case FutureStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				case FutureStatus.Pending:
					return FuturePool.Pull(promised, this);
				case FutureStatus.Resolved:
					FuturePool.Release(this);
					return promised.Invoke();
				default:
					return this;
			}
		}

		public IFuture<T> Then<T>(FutureResultPromised<T> promised)
		{
			switch (_status)
			{
				case FutureStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				case FutureStatus.Pending:
					return FuturePool.Pull(promised, this);
				case FutureStatus.Resolved:
					FuturePool.Release(this);
					return promised.Invoke();
				default:
					Error reason = _reason;
					FuturePool.Release(this);
					return FuturePool.PullRejected<T>(reason);
			}
		}

		public void Then(FutureResolved resolved, FutureRejected rejected)
		{
			switch (_status)
			{
				case FutureStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				case FutureStatus.Pending:
					_resolved += resolved;
					break;
				case FutureStatus.Resolved:
					resolved.Invoke();
					return;
			}
			Catch(rejected);
		}

		public void Catch(FutureRejected rejected)
		{
			switch (_status)
			{
				case FutureStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				case FutureStatus.Pending:
					_rejected += rejected;
					break;
				case FutureStatus.Rejected:
					rejected.Invoke(_reason);
					break;
			}
		}

		public void Finally(IFutureHandler finalizer)
		{
			if (finalizer == null)
			{
				throw new ArgumentNullException(nameof(finalizer));
			}
			if (_finalizer != null)
			{
				throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
						"Future already has finalizer.");
			}
			switch (_status)
			{
				case FutureStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				case FutureStatus.Pending:
					_finalizer = finalizer;
					break;
				case FutureStatus.Resolved:
				case FutureStatus.Rejected:
					finalizer.HandleFuture(this);
					break;
			}
		}

		public void Reject(Error reason)
		{
			switch (_status)
			{
				case FutureStatus.Resolved:
				case FutureStatus.Rejected:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future already resolved or rejected.");
				default:
					_status = FutureStatus.Rejected;
					_reason = reason;
					HandleReject();
					HandleClear();
					break;
			}
		}

		protected void Init()
		{
			if (_status != FutureStatus.Empty)
			{
				throw new ReuseException(this);
			}
			_status = FutureStatus.Pending;
		}

		protected virtual void HandleClear()
		{
			_resolved = null;
			_rejected = null;
			_finalizer = null;
		}

		protected virtual void HandleResolve()
		{
			if (_resolved != null || _finalizer != null)
			{
				FutureResolved? resolved = _resolved;
				IFutureHandler? finalizer = _finalizer;
				resolved?.Invoke();
				finalizer?.HandleFuture(this);
			}
		}

		private void HandleReject()
		{
			if (_rejected != null || _finalizer != null)
			{
				Error reason = _reason;
				FutureRejected? rejected = _rejected;
				IFutureHandler? finalizer = _finalizer;
				rejected?.Invoke(reason);
				finalizer?.HandleFuture(this);
			}
		}
	}
}