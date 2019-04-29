using System;

namespace Chlorine.Pools
{
	public static class PromisePool
	{
		private static readonly Type PromiseType = typeof(Promise);

		public static void Clear()
		{
			SharedPool.Clear(PromiseType);
		}

		public static Promise Pull()
		{
			return SharedPool.Pull<Promise>() ?? new Promise();
		}

		public static void Release(IPromise promise)
		{
			if (promise == null)
			{
				throw new ArgumentNullException(nameof(promise));
			}
			SharedPool.UnsafeRelease(PromiseType, promise, true);
		}
	}

	public static class PromisePool<TResult>
	{
		private static readonly Type PromiseType = typeof(Promise<TResult>);

		public static void Clear()
		{
			SharedPool.Clear(PromiseType);
		}

		public static Promise<TResult> Pull()
		{
			return SharedPool.Pull<Promise<TResult>>() ?? new Promise<TResult>();
		}

		public static void Release(IPromise<TResult> promise)
		{
			if (promise == null)
			{
				throw new ArgumentNullException(nameof(promise));
			}
			SharedPool.UnsafeRelease(PromiseType, promise, true);
		}
	}
}