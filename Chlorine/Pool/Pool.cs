using System.Collections.Generic;

namespace Chlorine
{
	internal class Pool<T>
	{
		private readonly Stack<T> _stack = new Stack<T>();

		public T Pull()
		{
			if (_stack.Count > 0)
			{
				return _stack.Pop();
			}
			return default;
		}

		public void Release(T value)
		{
			_stack.Push(value);
		}
	}
}