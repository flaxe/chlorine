using Chlorine.Exceptions;
using Chlorine.Futures.Internal;
using Chlorine.Pools;

namespace Chlorine.Futures
{
	internal sealed class PromiseFuture : AbstractFuture, IFutureHandler
	{
		private FuturePromised _promised;

		private IFuture _parent;
		private IFuture _internal;

		internal void Init(FuturePromised promised, IFuture parent)
		{
			if (_parent != null || _internal != null)
			{
				throw new ReuseException(this);
			}
			_promised = promised;
			_parent = parent;
			_parent.Finally(this);
		}

		public override void Reset()
		{
			base.Reset();
			if (_parent != null)
			{
				SharedPool.UnsafeRelease(_parent.GetType(), _parent, true);
				_parent = null;
			}
			if (_internal != null)
			{
				SharedPool.UnsafeRelease(_internal.GetType(), _internal, true);
				_internal = null;
			}
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			_promised = null;
		}

		void IFutureHandler.HandleFuture(IFuture future)
		{
			if (future.IsRejected)
			{
				Reject(future.Reason);
			}
			else if (future == _parent && _internal == null)
			{
				_internal = _promised.Invoke();
				_internal.Finally(this);
				_promised = null;
			}
			else if (future == _internal)
			{
				Resolve();
			}
			else
			{
				throw new ChlorineException(ChlorineErrorCode.InvalidState,
						"Invalid state. Unexpected future.");
			}
		}
	}

	internal sealed class PromiseFuture<TInput> : AbstractFuture, IFutureHandler
	{
		private FuturePromised<TInput> _promised;

		private IFuture<TInput> _parent;
		private IFuture _internal;

		internal void Init(FuturePromised<TInput> promised, IFuture<TInput> parent)
		{
			if (_parent != null || _internal != null)
			{
				throw new ReuseException(this);
			}
			_promised = promised;
			_parent = parent;
			_parent.Finally(this);
		}

		public override void Reset()
		{
			base.Reset();
			if (_parent != null)
			{
				SharedPool.UnsafeRelease(_parent.GetType(), _parent, true);
				_parent = null;
			}
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			if (_internal != null)
			{
				SharedPool.UnsafeRelease(_internal.GetType(), _internal, true);
				_internal = null;
			}
			_promised = null;
		}

		void IFutureHandler.HandleFuture(IFuture future)
		{
			if (future.IsRejected)
			{
				Reject(future.Reason);
			}
			else if (future == _parent && _parent.TryGetResult(out TInput input) && _internal == null)
			{
				_internal = _promised.Invoke(input);
				_internal.Finally(this);
				_promised = null;
			}
			else if (future == _internal)
			{
				Resolve();
			}
			else
			{
				throw new ChlorineException(ChlorineErrorCode.InvalidState,
						"Invalid state. Unexpected future.");
			}
		}
	}

	internal sealed class PromiseFutureResult<TOutput> : AbstractFuture<TOutput>, IFutureHandler
	{
		private FutureResultPromised<TOutput> _promised;

		private IFuture _parent;
		private IFuture<TOutput> _internal;

		internal void Init(FutureResultPromised<TOutput> promised, IFuture parent)
		{
			if (_parent != null || _internal != null)
			{
				throw new ReuseException(this);
			}
			_promised = promised;
			_parent = parent;
			_parent.Finally(this);
		}

		public override void Reset()
		{
			base.Reset();
			if (_parent != null)
			{
				SharedPool.UnsafeRelease(_parent.GetType(), _parent, true);
				_parent = null;
			}
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			if (_internal != null)
			{
				SharedPool.UnsafeRelease(_internal.GetType(), _internal, true);
				_internal = null;
			}
			_promised = null;
		}

		void IFutureHandler.HandleFuture(IFuture future)
		{
			if (future.IsRejected)
			{
				Reject(future.Reason);
			}
			else if (future == _parent && _internal == null)
			{
				_internal = _promised.Invoke();
				_internal.Finally(this);
				_promised = null;
			}
			else if (future == _internal && _internal.TryGetResult(out TOutput output))
			{
				Resolve(output);
			}
			else
			{
				throw new ChlorineException(ChlorineErrorCode.InvalidState,
						"Invalid state. Unexpected future.");
			}
		}
	}

	internal sealed class PromiseFutureResult<TOutput, TInput> : AbstractFuture<TOutput>, IFutureHandler
	{
		private FutureResultPromised<TOutput, TInput> _promised;

		private IFuture<TInput> _parent;
		private IFuture<TOutput> _internal;

		internal void Init(FutureResultPromised<TOutput, TInput> promised, IFuture<TInput> parent)
		{
			if (_parent != null || _internal != null)
			{
				throw new ReuseException(this);
			}
			_promised = promised;
			_parent = parent;
			_parent.Finally(this);
		}

		public override void Reset()
		{
			base.Reset();
			if (_parent != null)
			{
				SharedPool.UnsafeRelease(_parent.GetType(), _parent, true);
				_parent = null;
			}
			if (_internal != null)
			{
				SharedPool.UnsafeRelease(_internal.GetType(), _internal, true);
				_internal = null;
			}
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			_promised = null;
		}

		void IFutureHandler.HandleFuture(IFuture future)
		{
			if (future.IsRejected)
			{
				Reject(future.Reason);
			}
			else if (future == _parent && _parent.TryGetResult(out TInput input) && _internal == null)
			{
				_internal = _promised.Invoke(input);
				_internal.Finally(this);
				_promised = null;
			}
			else if (future == _internal && _internal.TryGetResult(out TOutput output))
			{
				Resolve(output);
			}
			else
			{
				throw new ChlorineException(ChlorineErrorCode.InvalidState,
						"Invalid state. Unexpected future.");
			}
		}
	}
}