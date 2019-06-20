using System;
using Chlorine.Futures;

namespace Chlorine.Pools
{
	public static class PromisePool
	{
		public static Promise Pull()
		{
			return SharedPool.Pull<Promise>() ?? new Promise();
		}

		public static Promise<TResult> Pull<TResult>()
		{
			return SharedPool.Pull<Promise<TResult>>() ?? new Promise<TResult>();
		}

		public static void Release(IPromise promise)
		{
			if (promise == null)
			{
				throw new ArgumentNullException(nameof(promise));
			}
			SharedPool.UnsafeRelease(promise.GetType(), promise, true);
		}
	}
}