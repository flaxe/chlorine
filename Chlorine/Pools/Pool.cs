using System;

namespace Chlorine.Pools
{
	public static class Pool<T> where T : class, new()
	{
		public static void Clear()
		{
			SharedPool.Clear(typeof(T));
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
			SharedPool.UnsafeRelease(value.GetType(), value, reset);
		}
	}
}