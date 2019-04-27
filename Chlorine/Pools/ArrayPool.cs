using System;
using System.Collections.Generic;

namespace Chlorine
{
	public static class ArrayPool<T>
	{
		private static readonly List<Stack<T[]>> StackByLength = new List<Stack<T[]>>();

		public static void Clear()
		{
			StackByLength.Clear();
		}

		public static T[] Pull(int length)
		{
			Stack<T[]> stack = ResolveStack(length);
			return stack.Count > 0 ? stack.Pop() : new T[length];
		}

		public static void Release(T[] array, bool clear = true)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}
			int length = array.Length;
			if (clear)
			{
				Array.Clear(array, 0, length);
			}
			ResolveStack(length).Push(array);
		}

		private static Stack<T[]> ResolveStack(int length)
		{
			Stack<T[]> stack;
			int capacity = length + 1;
			if (StackByLength.Count < capacity)
			{
				StackByLength.Capacity = capacity;
				for (int i = StackByLength.Count; i < capacity; i++)
				{
					if (i == length)
					{
						stack = new Stack<T[]>();
						StackByLength.Add(stack);
						return stack;
					}
					StackByLength.Add(null);
				}
			}
			stack = StackByLength[length];
			if (stack != null)
			{
				return stack;
			}
			stack = new Stack<T[]>();
			StackByLength[length] = stack;
			return stack;
		}
	}
}