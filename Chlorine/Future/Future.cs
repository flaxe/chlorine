namespace Chlorine
{
	public class Future : AbstractFuture, IFuture
	{
		private IPromise _promise;

		public Future(IPromise promise)
		{
			Join(promise);
		}

		public override void Clear()
		{
			base.Clear();
			if (_promise != null)
			{
				_promise.Revoke(this);
				_promise = null;
			}
		}

		public void Join(IPromise promise)
		{
			_promise?.Revoke(this);
			_promise = promise;
			_promise.Fulfill(this);
		}

		public void Resolve()
		{
			if (Status == FutureStatus.Pending)
			{
				Status = FutureStatus.Resolved;
				HandleResolve();
				Clear();
			}
		}
	}

	public class Future<TResult> : AbstractFuture, IFuture<TResult>
	{
		private FutureResolved<TResult> _resultResolved;

		private TResult _result;

		private IPromise<TResult> _promise;

		public Future(IPromise<TResult> promise)
		{
			Join(promise);
		}

		public TResult Result => _result;

		public override void Reset()
		{
			base.Reset();
			_result = default;
		}

		public override void Clear()
		{
			base.Clear();
			_resultResolved = null;
			if (_promise != null)
			{
				_promise.Revoke(this);
				_promise = null;
			}
		}

		public void Join(IPromise<TResult> promise)
		{
			_promise?.Revoke(this);
			_promise = promise;
			_promise.Fulfill(this);
		}

		public void Then(FutureResolved<TResult> resolved, FutureRejected rejected)
		{
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

		public void Resolve(TResult result)
		{
			if (Status == FutureStatus.Pending)
			{
				Status = FutureStatus.Resolved;
				_result = result;
				HandleResolve();
				Clear();
			}
		}

		protected override void HandleResolve()
		{
			base.HandleResolve();
			_resultResolved?.Invoke(_result);
		}
	}
}