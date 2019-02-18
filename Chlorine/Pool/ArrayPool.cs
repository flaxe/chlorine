using System.Collections.Generic;

namespace Chlorine
{
	public class ArrayPool<T>
	{
		private readonly List<Pool<T[]>> _poolByLength = new List<Pool<T[]>>();

		public T[] Pull(int length)
		{
			return ResolvePool(length).Pull();
		}

		public void Release(T[] array)
		{
			int length = array.Length;
			for (int i = 0; i < length; i++)
			{
				array[i] = default;
			}
			ResolvePool(length).Release(array);
		}

		private Pool<T[]> ResolvePool(int length)
		{
			Pool<T[]> pool;
			int poolCapacity = length + 1;
			if (_poolByLength.Count < poolCapacity)
			{
				_poolByLength.Capacity = poolCapacity;
				for (int i = _poolByLength.Count; i < poolCapacity; i++)
				{
					if (i == length)
					{
						pool = new Pool<T[]>();
						_poolByLength.Add(pool);
						return pool;
					}

					_poolByLength.Add(null);
				}
			}

			pool = _poolByLength[length];
			if (pool != null)
			{
				return pool;
			}

			pool = new Pool<T[]>();
			_poolByLength[length] = pool;
			return pool;
		}
	}
}