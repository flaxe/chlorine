using System;

namespace Chlorine.Pools
{
	public static class Pool<T> where T : class, new()
	{
		private static readonly Type Type = typeof(T);

		public static void Clear()
		{
			SharedPool.Clear(Type);
		}

		public static T Pull()
		{
			T value = SharedPool.Pull<T>();
			if (value != null)
			{
				return value;
			}
			return new T();
		}

		public static void Release(T value, bool reset = true)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			SharedPool.UnsafeRelease(Type, value, reset);
		}
	}
}