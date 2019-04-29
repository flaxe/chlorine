using System;

namespace Chlorine.Pools
{
	public static class WeakReferencePool<T> where T : class
	{
		private static readonly Type WeakReferenceType = typeof(WeakReference<T>);

		public static void Clear()
		{
			SharedPool.Clear(WeakReferenceType);
		}

		public static WeakReference<T> Pull(T target)
		{
			WeakReference<T> reference = SharedPool.Pull<WeakReference<T>>();
			if (reference != null)
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
			SharedPool.UnsafeRelease(WeakReferenceType, reference, true);
		}
	}
}