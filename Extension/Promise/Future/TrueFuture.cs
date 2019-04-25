namespace Chlorine
{
	internal class TrueFuture : IFuture, IPoolable
	{
		internal TrueFuture()
		{
		}

		public bool IsResolved => true;
		public bool IsRejected => false;

		public Error Reason => throw new FutureException("Invalid operation. Future was not rejected.");

		public bool TryGetReason(out Error reason)
		{
			reason = default;
			return false;
		}

		public void Reset()
		{
		}

		public void Clear()
		{
		}

		public void Then(FutureResolved resolved, FutureRejected rejected)
		{
			resolved.Invoke();
		}

		public void Catch(FutureRejected rejected)
		{
		}
	}

	internal class TrueFuture<TResult> : IFuture<TResult>, IPoolable
	{
		private enum FutureStatus
		{
			Pending,
			Resolved
		}

		private FutureStatus _status = FutureStatus.Pending;
		private TResult _result;

		internal TrueFuture()
		{
		}

		internal TrueFuture(TResult result)
		{
			Resolve(result);
		}

		public bool IsResolved => _status == FutureStatus.Resolved;
		public bool IsRejected => false;

		public Error Reason => throw new FutureException("Invalid operation. Future was not rejected.");

		public bool TryGetReason(out Error reason)
		{
			reason = default;
			return false;
		}

		public TResult Result
		{
			get
			{
				if (_status != FutureStatus.Resolved)
				{
					throw new FutureException("Invalid operation. Future was not resolved.");
				}
				return _result;
			}
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

		public void Reset()
		{
			_status = FutureStatus.Pending;
			_result = default;
		}

		public void Clear()
		{
		}

		public void Then(FutureResolved resolved, FutureRejected rejected)
		{
			switch (_status)
			{
				case FutureStatus.Resolved:
					resolved.Invoke();
					break;
				case FutureStatus.Pending:
					throw new FutureException("Invalid operation. TrueFuture must be resolved before usage.");
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
					throw new FutureException("Invalid operation. TrueFuture must be resolved before usage.");
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