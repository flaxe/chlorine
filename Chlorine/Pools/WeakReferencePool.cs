using System;

namespace Chlorine.Pools
{
	public static class WeakReferencePool<T> where T : class
	{
		private static readonly Type Type = typeof(WeakReference<T>);

		public static void Clear()
		{
			SharedPool.Clear(Type);
		}

		public static WeakReference<T> Pull(T target)
		{
			if (SharedPool.Pull(Type) is WeakReference<T> reference)
			{
				reference.SetTarget(target);
				return reference;
			}
			return new WeakReference<T>(target);
		}

		public static void Release(WeakReference<T> reference)
		{
			if (reference == null)
			{
				throw new ArgumentNullException(nameof(reference));
			}
			reference.SetTarget(null);
			SharedPool.UnsafeRelease(reference.GetType(), reference, true);
		}
	}
}