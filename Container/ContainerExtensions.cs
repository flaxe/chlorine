using System;
using Carbone.Pools;

namespace Carbone
{
	public static class ContainerExtensions
	{
		public static T Instantiate<T, T1>(this IContainer container, T1 argument1)
		{
			return Instantiate<T>(container, CreateArguments(argument1));
		}

		public static T Instantiate<T, T1, T2>(this IContainer container,
				T1 argument1,
				T2 argument2)
		{
			return Instantiate<T>(container, CreateArguments(argument1, argument2));
		}

		public static T Instantiate<T, T1, T2, T3>(this IContainer container,
				T1 argument1,
				T2 argument2,
				T3 argument3)
		{
			return Instantiate<T>(container, CreateArguments(argument1, argument2, argument3));
		}

		public static T Instantiate<T>(this IContainer container,
				object[] arguments)
		{
			return Instantiate<T>(container, CreateArguments(arguments));
		}

		public static object Instantiate<T1>(this IContainer container,
				Type type,
				T1 argument1)
		{
			return Instantiate(container, type, CreateArguments(argument1));
		}

		public static object Instantiate<T1, T2>(this IContainer container,
				Type type,
				T1 argument1,
				T2 argument2)
		{
			return Instantiate(container, type, CreateArguments(argument1, argument2));
		}

		public static object Instantiate<T1, T2, T3>(this IContainer container,
				Type type,
				T1 argument1,
				T2 argument2,
				T3 argument3)
		{
			return Instantiate(container, type, CreateArguments(argument1, argument2, argument3));
		}

		public static object Instantiate(this IContainer container,
				Type type,
				object[] arguments)
		{
			return Instantiate(container, type, CreateArguments(arguments));
		}

		public static void Inject<T1>(this IContainer container,
				object instance,
				T1 argument1)
		{
			Inject(container, instance, CreateArguments(argument1));
		}

		public static void Inject<T1, T2>(this IContainer container,
				object instance,
				T1 argument1,
				T2 argument2)
		{
			Inject(container, instance, CreateArguments(argument1, argument2));
		}

		public static void Inject<T1, T2, T3>(this IContainer container,
				object instance,
				T1 argument1,
				T2 argument2,
				T3 argument3)
		{
			Inject(container, instance, CreateArguments(argument1, argument2, argument3));
		}

		public static void Inject(this IContainer container,
				object instance,
				object[] arguments)
		{
			Inject(container, instance, CreateArguments(arguments));
		}

		private static T Instantiate<T>(IContainer container, TypeValue[] arguments)
		{
			T instance;
			try
			{
				instance = container.Instantiate<T>(arguments);
			}
			finally
			{
				ArrayPool<TypeValue>.Release(arguments);
			}
			return instance;
		}

		private static object Instantiate(IContainer container, Type type, TypeValue[] arguments)
		{
			object instance;
			try
			{
				instance = container.Instantiate(type, arguments);
			}
			finally
			{
				ArrayPool<TypeValue>.Release(arguments);
			}
			return instance;
		}

		private static void Inject(IContainer container, object instance, TypeValue[] arguments)
		{
			try
			{
				container.Inject(instance, arguments);
			}
			finally
			{
				ArrayPool<TypeValue>.Release(arguments);
			}
		}

		private static TypeValue[] CreateArguments<T1>(T1 argument1)
		{
			TypeValue[] arguments = ArrayPool<TypeValue>.Pull(1);
			arguments[0] = new TypeValue(typeof(T1), argument1!);
			return arguments;
		}

		private static TypeValue[] CreateArguments<T1, T2>(T1 argument1, T2 argument2)
		{
			TypeValue[] arguments = ArrayPool<TypeValue>.Pull(2);
			arguments[0] = new TypeValue(typeof(T1), argument1!);
			arguments[1] = new TypeValue(typeof(T2), argument2!);
			return arguments;
		}

		private static TypeValue[] CreateArguments<T1, T2, T3>(T1 argument1, T2 argument2, T3 argument3)
		{
			TypeValue[] arguments = ArrayPool<TypeValue>.Pull(3);
			arguments[0] = new TypeValue(typeof(T1), argument1!);
			arguments[1] = new TypeValue(typeof(T2), argument2!);
			arguments[2] = new TypeValue(typeof(T3), argument3!);
			return arguments;
		}

		private static TypeValue[] CreateArguments(object[] values)
		{
			TypeValue[] arguments = ArrayPool<TypeValue>.Pull(values.Length);
			for (int i = 0; i < values.Length; i++)
			{
				object argument = values[i];
				arguments[i] = new TypeValue(argument.GetType(), argument);
			}
			return arguments;
		}
	}
}