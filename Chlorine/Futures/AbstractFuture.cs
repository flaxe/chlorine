using System;
using System.Collections.Generic;
using Chlorine.Exceptions;
using Chlorine.Pools;

namespace Chlorine.Internal
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
				throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
						"Invalid operation. Future already resolved or rejected.");
			}
			Status = FutureStatus.Resolved;
			HandleResolve();
			HandleFinalize();
			Clear();
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
					throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
							"Invalid operation. Future was not resolved.");
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

		public override void Clear()
		{
			base.Clear();
			_resultResolved = null;
		}

		public IFuture Then(FuturePromised<TResult> promised)
		{
			PromiseFuture<TResult> future = Pool<PromiseFuture<TResult>>.Pull();
			future.Init(this, promised);
			return future;
		}

		public IFuture<T> Then<T>(FutureResultPromised<T, TResult> promised)
		{
			PromiseFutureResult<T, TResult> future = Pool<PromiseFutureResult<T, TResult>>.Pull();
			future.Init(this, promised);
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
				throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
						"Invalid operation. Future already resolved or rejected.");
			}
			Status = FutureStatus.Resolved;
			_result = result;
			HandleResolve();
			HandleFinalize();
			Clear();
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
				foreach (IFutureHandler<TResult> handler in _resultHandlers)
				{
					handler.HandleFuture(this);
				}
			}
		}
	}
}