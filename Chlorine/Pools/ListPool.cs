using System;
using System.Collections.Generic;

namespace Chlorine
{
	public sealed class ListPool<T>
	{
		private readonly List<List<T>> _lists = new List<List<T>>();

		public void Clear()
		{
			if (_lists.Count > 0)
			{
				_lists.Clear();
			}
		}

		public List<T> Pull(int capacity = 0)
		{
			if (_lists.Count > 0)
			{
				if (capacity > 0)
				{
					int index = GetIndex(capacity);
					if (index != -1)
					{
						List<T> list = _lists[index];
						_lists[index] = null;
						return list;
					}
				}
				else
				{
					for (int i = 0; i < _lists.Count; i++)
					{
						List<T> list = _lists[i];
						if (list != null)
						{
							_lists[i] = null;
							return list;
						}
					}
				}
			}
			return default;
		}

		public void Release(List<T> list, bool clear = true)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}
			if (clear)
			{
				list.Clear();
			}
			for (int i = 0; i < _lists.Count; i++)
			{
				if (_lists[i] == null)
				{
					_lists[i] = list;
					return;
				}
			}
			_lists.Add(list);
		}

		private int GetIndex(int capacity)
		{
			int index = -1;
			int capacityDifference = int.MaxValue;
			for (int i = 0; i < _lists.Count; i++)
			{
				List<T> list = _lists[i];
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