using System;
using System.Collections.Generic;

namespace Chlorine
{
	public sealed class Pool : IDisposable
	{
		private Dictionary<Type, Stack<object>> _stackByType;

		~Pool()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_stackByType != null && _stackByType.Count > 0)
			{
				Type disposableType = typeof(IDisposable);
				foreach (KeyValuePair<Type,Stack<object>> pair in _stackByType)
				{
					if (disposableType.IsAssignableFrom(pair.Key))
					{
						foreach (object value in pair.Value)
						{
							if (value is IDisposable disposable)
							{
								disposable.Dispose();
							}
						}
					}
				}
				_stackByType = null;
			}
		}

		public T Pull<T>() where T : class
		{
			if (_stackByType != null && _stackByType.TryGetValue(typeof(T), out Stack<object> stack) && stack.Count > 0)
			{
				return stack.Pop() as T;
			}
			return default;
		}

		public void Release(object value, bool reset = true)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			Type type = value.GetType();
			if (type.IsValueType || type.IsEnum)
			{
				throw new ArgumentException($"Type '{type.Name}' must be a reference type.");
			}
			if (reset && value is IPoolable poolable)
			{
				poolable.Reset();
			}
			if (_stackByType != null && _stackByType.TryGetValue(type, out Stack<object> stack))
			{
				stack.Push(value);
			}
			else
			{
				stack = new Stack<object>();
				stack.Push(value);
				if (_stackByType == null)
				{
					_stackByType = new Dictionary<Type, Stack<object>> {{type, stack}};
				}
				else
				{
					_stackByType.Add(type, stack);
				}
			}
		}
	}

	public sealed class Pool<T> : IDisposable where T : class
	{
		private Stack<T> _stack;

		~Pool()
		{
			Dispose();
		}

		public bool IsEmpty => _stack == null || _stack.Count == 0;

		public void Dispose()
		{
			if (_stack != null && _stack.Count > 0 && typeof(IDisposable).IsAssignableFrom(typeof(T)))
			{
				foreach (T value in _stack)
				{
					if (value is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
				_stack = null;
			}
		}

		public T Pull()
		{
			return _stack != null && _stack.Count > 0 ? _stack.Pop() : default;
		}

		public void Release(T value, bool reset = true)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			if (reset && value is IPoolable poolable)
			{
				poolable.Reset();
			}
			if (_stack == null)
			{
				_stack = new Stack<T>();
			}
			_stack.Push(value);
		}
	}
}