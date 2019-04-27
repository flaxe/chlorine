using System;
using System.Collections.Generic;

namespace Chlorine
{
	public static class ListPool<T>
	{
		private static readonly List<List<T>> Lists = new List<List<T>>();

		public static void Clear()
		{
			Lists.Clear();
		}

		public static List<T> Pull(int capacity = 0)
		{
			if (Lists.Count > 0)
			{
				if (capacity > 0)
				{
					int index = GetIndex(capacity);
					if (index != -1)
					{
						List<T> list = Lists[index];
						Lists[index] = null;
						return list;
					}
				}
				else
				{
					for (int i = 0; i < Lists.Count; i++)
					{
						List<T> list = Lists[i];
						if (list != null)
						{
							Lists[i] = null;
							return list;
						}
					}
				}
			}
			return new List<T>(capacity);
		}

		public static void Release(List<T> list, bool clear = true)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}
			if (clear)
			{
				list.Clear();
			}
			for (int i = 0; i < Lists.Count; i++)
			{
				if (Lists[i] == null)
				{
					Lists[i] = list;
					return;
				}
			}
			Lists.Add(list);
		}

		private static int GetIndex(int capacity)
		{
			int index = -1;
			int capacityDifference = int.MaxValue;
			for (int i = 0; i < Lists.Count; i++)
			{
				List<T> list = Lists[i];
				if (list == null || list.Capacity < capacity)
				{
					continue;
				}
				if (list.Capacity == capacity)
				{
					return index;
				}
				int difference = list.Capacity - capacity;
				if (difference < capacityDifference)
				{
					index = i;
					capacityDifference = difference;
				}
			}
			return index;
		}
	}
}