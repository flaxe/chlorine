using System;
using Chlorine;

namespace chlorine
{
	public class TrueFuture : IFuture, IPoolable
	{
		public TrueFuture()
		{
		}

		public bool IsResolved => true;
		public bool IsRejected => false;

		public void Reset()
		{
		}

		public void Clear()
		{
		}

		public bool TryGetReason(out Error reason)
		{
			reason = default;
			return false;
		}

		public void Then(FutureResolved resolved, FutureRejected rejected)
		{
			resolved.Invoke();
		}

		public void Catch(FutureRejected rejected)
		{
		}
	}

	public class TrueFuture<TResult> : IFuture<TResult>, IPoolable
	{
		private enum FutureStatus
		{
			Pending,
			Resolved
		}

		private FutureStatus _status = FutureStatus.Pending;
		private TResult _result;

		public TrueFuture()
		{
		}

		public TrueFuture(TResult result)
		{
			Resolve(result);
		}

		public bool IsResolved => _status == FutureStatus.Resolved;
		public bool IsRejected => false;

		public void Reset()
		{
			_status = FutureStatus.Pending;
			_result = default;
		}

		public void Clear()
		{
		}

		public bool TryGetReason(out Error reason)
		{
			reason = default;
			return false;
		}

		public bool TryGetResult(out TResult result)
		{
			if (_status == FutureStatus.Resolved)
			{
				result = _result;
				return true;
			}
			result = default;
			return false;
		}

		public void Then(FutureResolved resolved, FutureRejected rejected)
		{
			switch (_status)
			{
				case FutureStatus.Resolved:
					resolved.Invoke();
					break;
				case FutureStatus.Pending:
					throw new Exception("TrueFuture must be resolved before usage.");
			}
		}

		public void Then(FutureResolved<TResult> resolved, FutureRejected rejected)
		{
			switch (_status)
			{
				case FutureStatus.Resolved:
					resolved.Invoke(_result);
					break;
				case FutureStatus.Pending:
					throw new Exception("TrueFuture must be resolved before usage.");
			}
		}

		public void Catch(FutureRejected rejected)
		{
		}

		public void Resolve(TResult result)
		{
			if (_status == FutureStatus.Pending)
			{
				_status = FutureStatus.Resolved;
				_result = result;
			}
		}
	}
}