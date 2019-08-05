using System;
using System.Collections.Generic;

namespace Chlorine.Pools
{
	public static class SharedPool
	{
		private static readonly Dictionary<Type, Stack<object>> StackByType = new Dictionary<Type, Stack<object>>();

		public static void Clear()
		{
			StackByType.Clear();
		}

		public static void Clear<T>() where T : class
		{
			Clear(typeof(T));
		}

		public static void Clear(Type type)
		{
			StackByType.Remove(type);
		}

		internal static T Pull<T>() where T : class
		{
			object obj = Pull(typeof(T));
			if (obj != null && obj is T value)
			{
				return value;
			}
			return default;
		}

		internal static object Pull(Type type)
		{
			if (StackByType.TryGetValue(type, out Stack<object> stack) && stack.Count > 0)
			{
				return stack.Pop();
			}
			return default;
		}

		internal static void Release(object value, bool reset = true)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			Type type = value.GetType();
			if (type.IsValueType || type.IsEnum)
			{
				throw new ArgumentException($"'{type.Name}' must be a reference type.");
			}
			UnsafeRelease(type, value, reset);
		}

		internal static void UnsafeRelease(Type type, object value, bool reset)
		{
			if (reset && value is IPoolable poolable)
			{
				poolable.Reset();
			}
			if (StackByType.TryGetValue(type, out Stack<object> stack))
			{
				stack.Push(value);
			}
			else
			{
				stack = new Stack<object>();
				stack.Push(value);
				StackByType.Add(type, stack);
			}
		}
	}
}