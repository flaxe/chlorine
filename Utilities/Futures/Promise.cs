using Carbone.Collections;
using Carbone.Exceptions;
using Carbone.Futures.Internal;

namespace Carbone.Futures
{
	public sealed class Promise : AbstractFalsePromise
	{
		internal Promise()
		{
			Init();
		}

		public void Resolve()
		{
			switch (Status)
			{
				case PromiseStatus.Resolved:
				case PromiseStatus.Rejected:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise already resolved or rejected.");
				default:
					Status = PromiseStatus.Resolved;
					HandleResolve();
					HandleClear();
					break;
			}
		}
	}

	public sealed class Promise<TResult> : AbstractFalsePromise, IPromise<TResult>
	{
		private WeakReferenceList<Future<TResult>>? _resultFutures;

		private TResult _result;

		internal Promise()
		{
			_result = default!;
			Init();
		}

		public TResult Result
		{
			get
			{
				if (Status != PromiseStatus.Resolved)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise was not resolved.");
				}
				return _result;
			}
		}

		public bool TryGetResult(out TResult result)
		{
			if (Status == PromiseStatus.Resolved)
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

		public void Fulfill(Future<TResult> future)
		{
			switch (Status)
			{
				case PromiseStatus.Empty:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise is empty.");
				case PromiseStatus.Pending:
					if (_resultFutures == null)
					{
						_resultFutures = new WeakReferenceList<Future<TResult>>();
					}
					_resultFutures.Add(future);
					break;
				case PromiseStatus.Resolved:
					future.Resolve(_result);
					break;
				case PromiseStatus.Rejected:
					future.Reject(Reason);
					break;
			}
		}

		public void Revoke(Future<TResult> future)
		{
			_resultFutures?.Remove(future);
		}

		public void Resolve(TResult result)
		{
			switch (Status)
			{
				case PromiseStatus.Resolved:
				case PromiseStatus.Rejected:
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Promise already resolved or rejected.");
				default:
					Status = PromiseStatus.Resolved;
					_result = result;
					HandleResolve();
					HandleClear();
					break;
			}
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			_resultFutures?.Clear();
		}

		protected override void HandleResolve()
		{
			base.HandleResolve();
			if (_resultFutures != null && _resultFutures.Count > 0)
			{
				foreach (Future<TResult> resultFuture in _resultFutures)
				{
					resultFuture.Resolve(_result);
				}
			}
		}

		protected override void HandleReject()
		{
			base.HandleReject();
			if (_resultFutures != null && _resultFutures.Count > 0)
			{
				foreach (Future<TResult> resultFuture in _resultFutures)
				{
					resultFuture.Reject(Reason);
				}
			}
		}
	}
}