using System;
using System.Collections.Generic;
using Chlorine.Futures;

namespace Chlorine.Pools
{
	public static class FuturePool
	{
		public static void Clear()
		{
			SharedPool.Clear<Future>();
		}

		public static void Clear<TResult>()
		{
			SharedPool.Clear<Future<TResult>>();
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

		public static IFuture<TResult> Pull<TResult>(IPromise<TResult> promise)
		{
			Future<TResult> future = SharedPool.Pull<Future<TResult>>();
			if (future != null)
			{
				future.Init(promise);
				return future;
			}
			return new Future<TResult>(promise);
		}

		public static IFuture Pull(IEnumerable<IFuture> futures)
		{
			FutureAll future = SharedPool.Pull<FutureAll>();
			if (future != null)
			{
				future.Init(futures);
				return future;
			}
			return new FutureAll(futures);
		}

		public static IFuture PullResolved()
		{
			Future future = SharedPool.Pull<Future>();
			if (future != null)
			{
				future.Resolve();
				return future;
			}
			return new Future();
		}

		public static IFuture<TResult> PullResolved<TResult>(TResult result)
		{
			Future<TResult> future = SharedPool.Pull<Future<TResult>>();
			if (future != null)
			{
				future.Resolve(result);
				return future;
			}
			return new Future<TResult>(result);
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

		public static IFuture<TResult> PullRejected<TResult>(Error reason)
		{
			Future<TResult> future = SharedPool.Pull<Future<TResult>>();
			if (future != null)
			{
				future.Reject(reason);
				return future;
			}
			return new Future<TResult>(reason);
		}

		public static void Release(IFuture future)
		{
			if (future == null)
			{
				throw new ArgumentNullException(nameof(future));
			}
			SharedPool.UnsafeRelease(future.GetType(), future, true);
		}
	}
}