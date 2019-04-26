using System;
using System.Collections.Generic;

namespace Chlorine
{
	public sealed class ArrayPool<T>
	{
		private readonly List<Stack<T[]>> _stackByLength = new List<Stack<T[]>>();

		public void Clear()
		{
			if (_stackByLength.Count > 0)
			{
				_stackByLength.Clear();
			}
		}

		public T[] Pull(int length)
		{
			Stack<T[]> stack = ResolveStack(length);
			return stack.Count > 0 ? stack.Pop() : default;
		}

		public void Release(T[] array, bool clear = true)
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

		private Stack<T[]> ResolveStack(int length)
		{
			Stack<T[]> stack;
			int capacity = length + 1;
			if (_stackByLength.Count < capacity)
			{
				_stackByLength.Capacity = capacity;
				for (int i = _stackByLength.Count; i < capacity; i++)
				{
					if (i == length)
					{
						stack = new Stack<T[]>();
						_stackByLength.Add(stack);
						return stack;
					}
					_stackByLength.Add(null);
				}
			}
			stack = _stackByLength[length];
			if (stack != null)
			{
				return stack;
			}
			stack = new Stack<T[]>();
			_stackByLength[length] = stack;
			return stack;
		}
	}
}