using System;
using System.Collections.Generic;
using Chlorine.Futures;

namespace Chlorine.Pools
{
	public static class PromisePool
	{
		private static readonly HashSet<Type> PromiseTypes = new HashSet<Type>();

		public static void Clear()
		{
			foreach (Type promiseType in PromiseTypes)
			{
				SharedPool.Clear(promiseType);
			}
			PromiseTypes.Clear();
		}

		public static Promise Pull()
		{
			if (SharedPool.Pull(typeof(Promise)) is Promise promise)
			{
				promise.Init();
				return promise;
			}
			return new Promise();
		}

		public static Promise<TResult> Pull<TResult>()
		{
			if (SharedPool.Pull(typeof(Promise<TResult>)) is Promise<TResult> promise)
			{
				promise.Init();
				return promise;
			}
			return new Promise<TResult>();
		}

		public static void Release(IPromise promise)
		{
			if (promise == null)
			{
				throw new ArgumentNullException(nameof(promise));
			}
			Type promiseType = promise.GetType();
			PromiseTypes.Add(promiseType);
			SharedPool.UnsafeRelease(promiseType, promise, true);
		}
	}
}