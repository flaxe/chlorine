using System;
using System.Collections.Generic;
using Chlorine.Exceptions;
using Chlorine.Pools;

namespace Chlorine.Futures.Internal
{
	public abstract class AbstractFuture : AbstractFalseFuture
	{
		protected AbstractFuture()
		{
		}

		protected AbstractFuture(Error reason) : base(reason)
		{
		}

		public void Resolve()
		{
			if (Status != FutureStatus.Pending)
			{
				throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
						"Future already resolved or rejected.");
			}
			Status = FutureStatus.Resolved;
			HandleResolve();
			HandleFinalize();
			HandleClear();
		}
	}

	public abstract class AbstractFuture<TResult> : AbstractFalseFuture, IFuture<TResult>
	{
		private FutureResolved<TResult> _resultResolved;

		private List<IFutureHandler<TResult>> _resultHandlers;

		private TResult _result;

		protected AbstractFuture()
		{
		}

		protected AbstractFuture(TResult result)
		{
			Status = FutureStatus.Resolved;
			_result = result;
		}

		protected AbstractFuture(Error reason) : base(reason)
		{
		}

		public TResult Result
		{
			get
			{
				if (Status != FutureStatus.Resolved)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future was not resolved.");
				}
				return _result;
			}
		}

		public bool TryGetResult(out TResult result)
		{
			if (Status == FutureStatus.Resolved)
			{
				result = _result;
				return true;
			}
			result = default;
			return false;
		}

		public override void Reset()
		{
			base.Reset();
			_result = default;
		}

		public IFuture Then(FuturePromised<TResult> promised)
		{
			if (!(SharedPool.Pull(typeof(PromiseFuture<TResult>)) is PromiseFuture<TResult> future))
			{
				future = new PromiseFuture<TResult>();
			}
			future.Init(promised, this);
			return future;
		}

		public IFuture<T> Then<T>(FutureResultPromised<T, TResult> promised)
		{
			if (!(SharedPool.Pull(typeof(PromiseFutureResult<T, TResult>)) is PromiseFutureResult<T, TResult> future))
			{
				future = new PromiseFutureResult<T, TResult>();
			}
			future.Init(promised, this);
			return future;
		}

		public void Then(FutureResolved<TResult> resolved, FutureRejected rejected)
		{
			if (resolved == null)
			{
				throw new ArgumentNullException(nameof(resolved));
			}
			switch (Status)
			{
				case FutureStatus.Pending:
					_resultResolved += resolved;
					break;
				case FutureStatus.Resolved:
					resolved.Invoke(_result);
					break;
			}
			Catch(rejected);
		}

		public void Finally(IFutureHandler<TResult> handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}
			switch (Status)
			{
				case FutureStatus.Pending:
					if (_resultHandlers == null)
					{
						_resultHandlers = new List<IFutureHandler<TResult>>{ handler };
					}
					else
					{
						_resultHandlers.Add(handler);
					}
					break;
				case FutureStatus.Resolved:
				case FutureStatus.Rejected:
					handler.HandleFuture(this);
					break;
			}
		}

		public void Resolve(TResult result)
		{
			if (Status != FutureStatus.Pending)
			{
				throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
						"Future already resolved or rejected.");
			}
			Status = FutureStatus.Resolved;
			_result = result;
			HandleResolve();
			HandleFinalize();
			HandleClear();
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			_resultResolved = null;
			_resultHandlers?.Clear();
		}

		protected override void HandleResolve()
		{
			base.HandleResolve();
			_resultResolved?.Invoke(_result);
		}

		protected override void HandleFinalize()
		{
			base.HandleFinalize();
			if (_resultHandlers != null && _resultHandlers.Count > 0)
			{
				List<IFutureHandler<TResult>> handlers = ListPool<IFutureHandler<TResult>>.Pull(_resultHandlers);
				foreach (IFutureHandler<TResult> handler in handlers)
				{
					handler.HandleFuture(this);
				}
				ListPool<IFutureHandler<TResult>>.Release(handlers);
			}
		}
	}
}