using System;
using System.Collections.Generic;

namespace Carbone.Pools
{
	public static class ListPool<T>
	{
		private static Stack<List<T>>[] _stacks = Array.Empty<Stack<List<T>>>();

		public static void Clear()
		{
			Array.Clear(_stacks, 0, _stacks.Length);
		}

		public static List<T> Pull(IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}
			List<T> list = enumerable is ICollection<T> collection ? Pull(collection.Count, true) : Pull();
			list.AddRange(enumerable);
			return list;
		}

		public static List<T> Pull(int capacity = 0, bool strict = false)
		{
			if (capacity > 0)
			{
				int index = capacity - 1;
				if (index < _stacks.Length)
				{
					Stack<List<T>> stack = _stacks[index];
					if (stack != null && stack.Count > 0)
					{
						return stack.Pop();
					}
				}
			}
			if (!strict)
			{
				for (int i = capacity; i < _stacks.Length; i++)
				{
					Stack<List<T>> stack = _stacks[i];
					if (stack != null && stack.Count > 0)
					{
						return stack.Pop();
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
			int capacity = list.Capacity;
			if (capacity == 0)
			{
				return;
			}
			if (clear)
			{
				list.Clear();
			}
			int index = capacity - 1;
			if (index >= _stacks.Length)
			{
				int stackLength = Math.Max(_stacks.Length * 2, index + 1);
				Stack<List<T>>[] stacks = new Stack<List<T>>[stackLength];
				Array.Copy(_stacks, 0, stacks, 0, _stacks.Length);
				_stacks = stacks;
			}
			Stack<List<T>> stack = _stacks[index];
			if (stack == null)
			{
				stack = new Stack<List<T>>();
				_stacks[index] = stack;
			}
			stack.Push(list);
		}
	}
}