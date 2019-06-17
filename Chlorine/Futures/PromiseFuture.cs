using System;
using Chlorine.Exceptions;
using Chlorine.Internal;
using Chlorine.Pools;

namespace Chlorine
{
	internal sealed class PromiseFuture : AbstractFuture, IFutureHandler
	{
		private static readonly Type FutureType = typeof(PromiseFuture);

		private IFuture _parent;
		private FuturePromised _promised;
		private IFuture _future;

		internal void Init(IFuture parent, FuturePromised promised)
		{
			if (_parent != null)
			{
				throw new ReuseException(FutureType.Name);
			}
			_parent = parent;
			_promised = promised;
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

		public override void Clear()
		{
			base.Clear();
			if (_future != null)
			{
				SharedPool.UnsafeRelease(_future.GetType(), _future, true);
				_future = null;
			}
			_promised = null;
		}

		void IFutureHandler.HandleFuture(IFuture future)
		{
			if (future.IsRejected)
			{
				Reject(future.Reason);
			}
			else if (future == _parent && _future == null)
			{
				_future = _promised.Invoke();
				_future.Finally(this);
			}
			else if (future == _future)
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
		private static readonly Type FutureType = typeof(PromiseFuture<TInput>);

		private IFuture<TInput> _parent;
		private FuturePromised<TInput> _promised;
		private IFuture _future;

		internal void Init(IFuture<TInput> parent, FuturePromised<TInput> promised)
		{
			if (_parent != null)
			{
				throw new ReuseException(FutureType.Name);
			}
			_parent = parent;
			_promised = promised;
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

		public override void Clear()
		{
			base.Clear();
			if (_future != null)
			{
				SharedPool.UnsafeRelease(_future.GetType(), _future, true);
				_future = null;
			}
			_promised = null;
		}

		public void HandleFuture(IFuture future)
		{
			if (future.IsRejected)
			{
				Reject(future.Reason);
			}
			else if (future == _parent && _parent.TryGetResult(out TInput input) && _future == null)
			{
				_future = _promised.Invoke(input);
				_future.Finally(this);
			}
			else if (future == _future)
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
		private static readonly Type FutureType = typeof(PromiseFutureResult<TOutput>);

		private IFuture _parent;
		private FutureResultPromised<TOutput> _promised;
		private IFuture<TOutput> _future;

		internal void Init(IFuture parent, FutureResultPromised<TOutput> promised)
		{
			if (_parent != null)
			{
				throw new ReuseException(FutureType.Name);
			}
			_parent = parent;
			_promised = promised;
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

		public override void Clear()
		{
			base.Clear();
			if (_future != null)
			{
				SharedPool.UnsafeRelease(_future.GetType(), _future, true);
				_future = null;
			}
			_promised = null;
		}

		void IFutureHandler.HandleFuture(IFuture future)
		{
			if (future.IsRejected)
			{
				Reject(future.Reason);
			}
			else if (future == _parent && _future == null)
			{
				_future = _promised.Invoke();
				_future.Finally(this);
			}
			else if (future == _future && _future.TryGetResult(out TOutput output))
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
		private static readonly Type FutureType = typeof(PromiseFutureResult<TOutput, TInput>);

		private IFuture<TInput> _parent;
		private FutureResultPromised<TOutput, TInput> _promised;
		private IFuture<TOutput> _future;

		internal void Init(IFuture<TInput> parent, FutureResultPromised<TOutput, TInput> promised)
		{
			if (_parent != null)
			{
				throw new ReuseException(FutureType.Name);
			}
			_parent = parent;
			_promised = promised;
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

		public override void Clear()
		{
			base.Clear();
			if (_future != null)
			{
				SharedPool.UnsafeRelease(_future.GetType(), _future, true);
				_future = null;
			}
			_promised = null;
		}

		public void HandleFuture(IFuture future)
		{
			if (future.IsRejected)
			{
				Reject(future.Reason);
			}
			else if (future == _parent && _parent.TryGetResult(out TInput input) && _future == null)
			{
				_future = _promised.Invoke(input);
				_future.Finally(this);
			}
			else if (future == _future && _future.TryGetResult(out TOutput output))
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