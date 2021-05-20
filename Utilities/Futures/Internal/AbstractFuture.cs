using Carbone.Exceptions;

namespace Carbone.Futures.Internal
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
			switch (Status)
			{
				case FutureStatus.Resolved:
				case FutureStatus.Rejected:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future already resolved or rejected.");
				default:
					Status = FutureStatus.Resolved;
					HandleResolve();
					HandleClear();
					break;
			}
		}
	}

	public abstract class AbstractFuture<TResult> : AbstractFalseFuture, IFuture<TResult>
	{
		private FutureResolved<TResult>? _resultResolved;

		private TResult _result;

		protected AbstractFuture()
		{
			_result = default!;
		}

		protected AbstractFuture(TResult result)
		{
			Status = FutureStatus.Resolved;
			_result = result;
		}

		protected AbstractFuture(Error reason) : base(reason)
		{
			_result = default!;
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
			result = default!;
			return false;
		}

		public override void Reset()
		{
			base.Reset();
			_result = default!;
		}

		public IFuture Then(FuturePromised<TResult> promised)
		{
			switch (Status)
			{
				case FutureStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				case FutureStatus.Pending:
					return FuturePool.Pull(promised, this);
				case FutureStatus.Resolved:
					TResult result = _result;
					FuturePool.Release(this);
					return promised.Invoke(result);
				default:
					return this;
			}
		}

		public IFuture<T> Then<T>(FutureResultPromised<T, TResult> promised)
		{
			switch (Status)
			{
				case FutureStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				case FutureStatus.Pending:
					return FuturePool.Pull(promised, this);
				case FutureStatus.Resolved:
					TResult result = _result;
					FuturePool.Release(this);
					return promised.Invoke(result);
				default:
					Error reason = Reason;
					FuturePool.Release(this);
					return FuturePool.PullRejected<T>(reason);
			}
		}

		public void Then(FutureResolved<TResult> resolved, FutureRejected rejected)
		{
			switch (Status)
			{
				case FutureStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future is empty.");
				case FutureStatus.Pending:
					_resultResolved += resolved;
					break;
				case FutureStatus.Resolved:
					resolved.Invoke(_result);
					return;
			}
			Catch(rejected);
		}

		public void Resolve(TResult result)
		{
			switch (Status)
			{
				case FutureStatus.Resolved:
				case FutureStatus.Rejected:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Future already resolved or rejected.");
				default:
					Status = FutureStatus.Resolved;
					_result = result;
					HandleResolve();
					HandleClear();
					break;
			}
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			_resultResolved = null;
		}

		protected override void HandleResolve()
		{
			if (_resultResolved != null)
			{
				TResult result = _result;
				FutureResolved<TResult> resultResolved = _resultResolved;
				base.HandleResolve();
				resultResolved?.Invoke(result);
			}
			else
			{
				base.HandleResolve();
			}
		}
	}
}