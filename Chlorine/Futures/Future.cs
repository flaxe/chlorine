using Chlorine.Exceptions;
using Chlorine.Futures.Internal;

namespace Chlorine.Futures
{
	public sealed class Future : AbstractFuture
	{
		private IPromise _promise;

		internal Future(IPromise promise)
		{
			Init(promise);
		}

		internal Future()
		{
			Status = FutureStatus.Resolved;
		}

		internal Future(Error reason) : base(reason)
		{
		}

		internal void Init(IPromise promise)
		{
			if (_promise != null)
			{
				throw new ReuseException(this);
			}
			base.Init();
			_promise = promise;
			_promise.Fulfill(this);
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			if (_promise != null)
			{
				if (_promise.IsPending)
				{
					_promise.Revoke(this);
				}
				_promise = null;
			}
		}
	}

	public sealed class Future<TResult> : AbstractFuture<TResult>
	{
		private IPromise<TResult> _promise;

		internal Future(IPromise<TResult> promise)
		{
			Init(promise);
		}

		internal Future(TResult result) : base(result)
		{
		}

		internal Future(Error reason) : base(reason)
		{
		}

		internal void Init(IPromise<TResult> promise)
		{
			if (_promise != null)
			{
				throw new ReuseException(this);
			}
			base.Init();
			_promise = promise;
			_promise.Fulfill(this);
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			if (_promise != null)
			{
				if (_promise.IsPending)
				{
					_promise.Revoke(this);
				}
				_promise = null;
			}
		}
	}
}