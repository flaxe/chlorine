using System;
using System.Collections.Generic;

namespace Carbone.Pools
{
	public static class SharedPool
	{
		private static readonly Dictionary<Type, Stack<object>> StackByType = new Dictionary<Type, Stack<object>>();

		public static void Clear()
		{
			StackByType.Clear();
		}

		public static void Clear(Type type)
		{
			StackByType.Remove(type);
		}

		public static object? Pull(Type type)
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
				throw new ArgumentException($"\"{type.Name}\" must be a reference type.");
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