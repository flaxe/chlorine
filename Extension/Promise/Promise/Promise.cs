using Chlorine.Collections;
using Chlorine.Internal;

namespace Chlorine
{
	public class Promise : AbstractPromise, IPromise
	{
		internal Promise()
		{
		}

		public void Resolve()
		{
			if (Status == PromiseStatus.Pending)
			{
				Status = PromiseStatus.Resolved;
				HandleResolve();
				RevokeAll();
			}
		}
	}

	public class Promise<TResult> : AbstractPromise, IPromise<TResult>
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
					throw new PromiseException("Invalid operation. Promise was not resolved.");
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

		public override void RevokeAll()
		{
			base.RevokeAll();
			_resultFutures?.Clear();
		}

		public void Resolve(TResult result)
		{
			if (Status == PromiseStatus.Pending)
			{
				Status = PromiseStatus.Resolved;
				_result = result;
				HandleResolve();
				RevokeAll();
			}
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