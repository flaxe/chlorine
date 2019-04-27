using System;
using System.Collections.Generic;

namespace Chlorine
{
	public static class Pool<T> where T : class, new()
	{
		private static readonly Type Type = typeof(T);

		public static void Clear()
		{
			Pool.Clear(Type);
		}

		public static T Pull()
		{
			T value = Pool.Pull<T>();
			if (value != null)
			{
				return value;
			}
			return new T();
		}

		public static void Release(T value, bool reset = true)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			Pool.UnsafeRelease(Type, value, reset);
		}
	}

	internal static class Pool
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

		public static T Pull<T>() where T : class
		{
			object obj = Pull(typeof(T));
			if (obj != null && obj is T value)
			{
				return value;
			}
			return default;
		}

		public static object Pull(Type type)
		{
			if (StackByType.TryGetValue(type, out Stack<object> stack) && stack.Count > 0)
			{
				return stack.Pop();
			}
			return default;
		}

		public static void Release(object value, bool reset = true)
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
			UnsafeRelease(type, value, reset);
		}

		public static void UnsafeRelease(Type type, object value, bool reset)
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