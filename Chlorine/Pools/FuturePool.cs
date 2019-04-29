using System;

namespace Chlorine.Pools
{
	public static class FuturePool
	{
		private static readonly Type FutureType = typeof(Future);

		public static void Clear()
		{
			SharedPool.Clear(FutureType);
		}

		public static IFuture Pull(IPromise promise)
		{
			Future future = SharedPool.Pull<Future>();
			if (future != null)
			{
				future.Init(promise);
				return future;
			}
			return new Future(promise);
		}

		public static IFuture PullResolved()
		{
			Future future = SharedPool.Pull<Future>() ?? new Future();
			future.Resolve();
			return future;
		}

		public static IFuture PullRejected(Error reason)
		{
			Future future = SharedPool.Pull<Future>();
			if (future != null)
			{
				future.Reject(reason);
				return future;
			}
			return new Future(reason);
		}

		public static void Release(IFuture future)
		{
			if (future == null)
			{
				throw new ArgumentNullException(nameof(future));
			}
			SharedPool.UnsafeRelease(FutureType, future, true);
		}
	}

	public static class FuturePool<TResult>
	{
		private static readonly Type FutureType = typeof(Future<TResult>);

		public static void Clear()
		{
			SharedPool.Clear(FutureType);
		}

		public static IFuture<TResult> Pull(IPromise<TResult> promise)
		{
			Future<TResult> future = SharedPool.Pull<Future<TResult>>();
			if (future != null)
			{
				future.Init(promise);
				return future;
			}
			return new Future<TResult>(promise);
		}

		public static IFuture<TResult> PullResolved(TResult result)
		{
			Future<TResult> future = SharedPool.Pull<Future<TResult>>();
			if (future != null)
			{
				future.Resolve(result);
				return future;
			}
			return new Future<TResult>(result);
		}

		public static IFuture<TResult> PullRejected(Error reason)
		{
			Future<TResult> future = SharedPool.Pull<Future<TResult>>();
			if (future != null)
			{
				future.Reject(reason);
				return future;
			}
			return new Future<TResult>(reason);
		}

		public static void Release(IFuture<TResult> future)
		{
			if (future == null)
			{
				throw new ArgumentNullException(nameof(future));
			}
			SharedPool.UnsafeRelease(FutureType, future, true);
		}
	}
}