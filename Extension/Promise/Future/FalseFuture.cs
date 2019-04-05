namespace Chlorine
{
	public class FalseFuture : IFuture, IPoolable
	{
		private enum FutureStatus
		{
			Pending,
			Rejected
		}

		private FutureStatus _status = FutureStatus.Pending;
		private Error _reason;

		internal FalseFuture()
		{
		}

		internal FalseFuture(Error reason)
		{
			Reject(reason);
		}

		public bool IsResolved => false;
		public bool IsRejected => _status == FutureStatus.Rejected;

		public void Reset()
		{
			_status = FutureStatus.Pending;
			_reason = default;
		}

		public void Clear()
		{
		}

		public bool TryGetReason(out Error reason)
		{
			if (_status == FutureStatus.Rejected)
			{
				reason = _reason;
				return true;
			}
			reason = default;
			return false;
		}

		public void Then(FutureResolved resolved, FutureRejected rejected)
		{
			Catch(rejected);
		}

		public void Catch(FutureRejected rejected)
		{
			switch (_status)
			{
				case FutureStatus.Rejected:
					rejected.Invoke(_reason);
					break;
				case FutureStatus.Pending:
					throw new FutureException("FalseFuture must be rejected before usage.");
			}
		}

		public void Reject(Error reason)
		{
			if (_status == FutureStatus.Pending)
			{
				_status = FutureStatus.Rejected;
				_reason = reason;
			}
		}
	}

	public class FalseFuture<TResult> : FalseFuture, IFuture<TResult>
	{
		internal FalseFuture()
		{
		}

		internal FalseFuture(Error reason) : base(reason)
		{
		}

		public bool TryGetResult(out TResult result)
		{
			result = default;
			return false;
		}

		public void Then(FutureResolved<TResult> resolved, FutureRejected rejected)
		{
			Catch(rejected);
		}
	}
}