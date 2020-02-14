using System;
using System.Collections.Generic;

namespace Chlorine.Pools
{
	public static class WeakReferencePool
	{
		private static readonly HashSet<Type> WeakReferenceTypes = new HashSet<Type>();

		public static void Clear()
		{
			foreach (Type futureType in WeakReferenceTypes)
			{
				SharedPool.Clear(futureType);
			}
			WeakReferenceTypes.Clear();
		}

		public static WeakReference<T> Pull<T>(T target)
				where T : class
		{
			if (SharedPool.Pull(typeof(T)) is WeakReference<T> reference)
			{
				reference.SetTarget(target);
				return reference;
			}
			return new WeakReference<T>(target);
		}

		public static void Release<T>(WeakReference<T> reference)
				where T : class
		{
			if (reference == null)
			{
				throw new ArgumentNullException(nameof(reference));
			}
			reference.SetTarget(null);
			Type referenceType = reference.GetType();
			WeakReferenceTypes.Add(referenceType);
			SharedPool.UnsafeRelease(referenceType, reference, true);
		}
	}
}