using System;
using System.Collections.Generic;
using Chlorine.Futures;

namespace Chlorine.Pools
{
	public static class FuturePool
	{
		private static readonly HashSet<Type> FutureTypes = new HashSet<Type>();

		public static void Clear()
		{
			foreach (Type futureType in FutureTypes)
			{
				SharedPool.ClearByType(futureType);
			}
			FutureTypes.Clear();
		}

		public static IFuture Pull(IPromise promise)
		{
			Type futureType = typeof(Future);
			if (SharedPool.Pull(futureType) is Future future)
			{
				future.Init(promise);
				return future;
			}
			return new Future(promise);
		}

		public static IFuture<TResult> Pull<TResult>(IPromise<TResult> promise)
		{
			Type futureType = typeof(Future<TResult>);
			if (SharedPool.Pull(futureType) is Future<TResult> future)
			{
				future.Init(promise);
				return future;
			}
			return new Future<TResult>(promise);
		}

		public static IFuture Pull(IEnumerable<IFuture> futures)
		{
			Type futureType = typeof(FutureAll);
			if (SharedPool.Pull(futureType) is FutureAll future)
			{
				future.Init(futures);
				return future;
			}
			return new FutureAll(futures);
		}

		public static IFuture PullResolved()
		{
			Type futureType = typeof(Future);
			if (SharedPool.Pull(futureType) is Future future)
			{
				future.Resolve();
				return future;
			}
			return new Future();
		}

		public static IFuture<TResult> PullResolved<TResult>(TResult result)
		{
			Type futureType = typeof(Future<TResult>);
			if (SharedPool.Pull(futureType) is Future<TResult> future)
			{
				future.Resolve(result);
				return future;
			}
			return new Future<TResult>(result);
		}

		public static IFuture PullRejected(Error reason)
		{
			Type futureType = typeof(Future);
			if (SharedPool.Pull(futureType) is Future future)
			{
				future.Reject(reason);
				return future;
			}
			return new Future(reason);
		}

		public static IFuture<TResult> PullRejected<TResult>(Error reason)
		{
			Type futureType = typeof(Future<TResult>);
			if (SharedPool.Pull(futureType) is Future<TResult> future)
			{
				future.Reject(reason);
				return future;
			}
			return new Future<TResult>(reason);
		}

		internal static IFuture Pull(FuturePromised promised, IFuture parent)
		{
			Type futureType = typeof(PromiseFuture);
			if (SharedPool.Pull(futureType) is PromiseFuture future)
			{
				future.Init(promised, parent);
				return future;
			}
			return new PromiseFuture(promised, parent);
		}

		internal static IFuture Pull<TResult>(FuturePromised<TResult> promised, IFuture<TResult> parent)
		{
			Type futureType = typeof(PromiseFuture<TResult>);
			if (SharedPool.Pull(futureType) is PromiseFuture<TResult> future)
			{
				future.Init(promised, parent);
				return future;
			}
			return new PromiseFuture<TResult>(promised, parent);
		}

		internal static IFuture<T> Pull<T>(FutureResultPromised<T> promised, IFuture parent)
		{
			Type futureType = typeof(PromiseFutureResult<T>);
			if (SharedPool.Pull(futureType) is PromiseFutureResult<T> future)
			{
				future.Init(promised, parent);
				return future;
			}
			return new PromiseFutureResult<T>(promised, parent);
		}

		internal static IFuture<T> Pull<T, TResult>(FutureResultPromised<T, TResult> promised, IFuture<TResult> parent)
		{
			Type futureType = typeof(PromiseFutureResult<T, TResult>);
			if (SharedPool.Pull(futureType) is PromiseFutureResult<T, TResult> future)
			{
				future.Init(promised, parent);
				return future;
			}
			return new PromiseFutureResult<T, TResult>(promised, parent);
		}

		public static void Release(IFuture future)
		{
			if (future == null)
			{
				throw new ArgumentNullException(nameof(future));
			}
			Type futureType = future.GetType();
			FutureTypes.Add(futureType);
			SharedPool.UnsafeRelease(futureType, future, true);
		}
	}
}