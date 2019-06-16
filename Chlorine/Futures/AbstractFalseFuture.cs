using System;
using System.Collections.Generic;
using Chlorine.Exceptions;
using Chlorine.Pools;

namespace Chlorine.Internal
{
	public abstract class AbstractFalseFuture : IFuture, IPoolable
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
		private List<IFutureHandler> _handlers;

		private Error _reason;

		protected AbstractFalseFuture()
		{
		}

		protected AbstractFalseFuture(Error reason)
		{
			_status = FutureStatus.Rejected;
			_reason = reason;
		}

		public bool IsResolved => _status == FutureStatus.Resolved;
		public bool IsRejected => _status == FutureStatus.Rejected;

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
					throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
							"Invalid operation. Future was not rejected.");
				}
				return _reason;
			}
		}

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
			_handlers?.Clear();
		}

		public IFuture Then(FuturePromised promised)
		{
			PromiseFuture future = Pool<PromiseFuture>.Pull();
			future.Init(this, promised);
			return future;
		}

		public IFuture<T> Then<T>(FutureResultPromised<T> promised)
		{
			PromiseFutureResult<T> future = Pool<PromiseFutureResult<T>>.Pull();
			future.Init(this, promised);
			return future;
		}

		public void Then(FutureResolved resolved, FutureRejected rejected)
		{
			if (resolved == null)
			{
				throw new ArgumentNullException(nameof(resolved));
			}
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
			if (rejected == null)
			{
				throw new ArgumentNullException(nameof(rejected));
			}
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

		public void Finally(IFutureHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}
			switch (_status)
			{
				case FutureStatus.Pending:
					if (_handlers == null)
					{
						_handlers = new List<IFutureHandler>{ handler };
					}
					else
					{
						_handlers.Add(handler);
					}
					break;
				case FutureStatus.Resolved:
				case FutureStatus.Rejected:
					handler.HandleFuture(this);
					break;
			}
		}

		public void Reject(Error reason)
		{
			if (_status != FutureStatus.Pending)
			{
				throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
						"Invalid operation. Future already resolved or rejected.");
			}
			_status = FutureStatus.Rejected;
			_reason = reason;
			HandleReject();
			HandleFinalize();
			Clear();
		}

		protected virtual void HandleResolve()
		{
			_resolved?.Invoke();
		}

		protected virtual void HandleFinalize()
		{
			if (_handlers != null && _handlers.Count > 0)
			{
				foreach (IFutureHandler handler in _handlers)
				{
					handler.HandleFuture(this);
				}
			}
		}

		private void HandleReject()
		{
			_rejected?.Invoke(_reason);
		}
	}
}