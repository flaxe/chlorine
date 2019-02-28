using System.Collections.Generic;

namespace Chlorine
{
	public class Pool<T> where T : class
	{
		private readonly Stack<T> _stack = new Stack<T>();

		public bool IsEmpty => _stack.Count == 0;

		public void Clear()
		{
			if (_stack.Count > 0)
			{
				_stack.Clear();
			}
		}

		public T Pull()
		{
			return _stack.Count > 0 ? _stack.Pop() : default;
		}

		public void Release(T value, bool reset = true)
		{
			if (reset && value is IPoolable poolable)
			{
				poolable.Reset();
			}
			_stack.Push(value);
		}
	}
}