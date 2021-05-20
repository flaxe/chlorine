using System;
using System.Collections.Generic;

namespace Carbone.Pools
{
	public static class ArrayPool<T>
	{
		private static Stack<T[]>[] _stacks = Array.Empty<Stack<T[]>>();

		public static void Clear()
		{
			Array.Clear(_stacks, 0, _stacks.Length);
		}

		public static T[] Pull(int length)
		{
			if (length > 0)
			{
				int index = length - 1;
				if (index < _stacks.Length)
				{
					Stack<T[]> stack = _stacks[index];
					if (stack != null && stack.Count > 0)
					{
						return stack.Pop();
					}
				}
			}
			return new T[length];
		}

		public static void Release(T[] array, bool clear = true)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}
			int length = array.Length;
			if (length == 0)
			{
				return;
			}
			if (clear)
			{
				Array.Clear(array, 0, length);
			}
			int index = length - 1;
			if (index >= _stacks.Length)
			{
				int stackLength = Math.Max(_stacks.Length * 2, index + 1);
				Stack<T[]>[] stacks = new Stack<T[]>[stackLength];
				Array.Copy(_stacks, 0, stacks, 0, _stacks.Length);
				_stacks = stacks;
			}
			Stack<T[]> stack = _stacks[index];
			if (stack == null)
			{
				stack = new Stack<T[]>();
				_stacks[index] = stack;
			}
			stack.Push(array);
		}
	}
}