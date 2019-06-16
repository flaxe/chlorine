using Chlorine.Collections;
using Chlorine.Exceptions;
using Chlorine.Internal;

namespace Chlorine
{
	public sealed class Promise : AbstractFalsePromise
	{
		internal Promise()
		{
		}

		public void Resolve()
		{
			if (Status != PromiseStatus.Pending)
			{
				throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
						"Invalid operation. Promise already resolved or rejected.");
			}
			Status = PromiseStatus.Resolved;
			HandleResolve();
			Clear();
		}
	}

	public sealed class Promise<TResult> : AbstractFalsePromise, IPromise<TResult>
	{
		private WeakReferenceList<Future<TResult>> _resultFutures;

		private TResult _result;

		internal Promise()
		{
		}

		public TResult Result
		{
			get
			{
				if (Status != PromiseStatus.Resolved)
				{
					throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
							"Invalid operation. Promise was not resolved.");
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
			result = default;
			return false;
		}

		public override void Reset()
		{
			base.Reset();
			_result = default;
		}

		public void Fulfill(Future<TResult> future)
		{
			switch (Status)
			{
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

		public override void Clear()
		{
			base.Clear();
			_resultFutures?.Clear();
		}

		public void Resolve(TResult result)
		{
			if (Status != PromiseStatus.Pending)
			{
				throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
						"Invalid operation. Promise already resolved or rejected.");
			}
			Status = PromiseStatus.Resolved;
			_result = result;
			HandleResolve();
			Clear();
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
	}
}