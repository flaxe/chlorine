using System;

namespace Chlorine
{
	public static class ContainerExtensions
	{
		private static readonly ArrayPool<Argument> ArgumentsPool = new ArrayPool<Argument>();

		public static T Instantiate<T, T1>(this IContainer container, T1 argument1)
		{
			return Instantiate<T>(container, CreateArguments(argument1));
		}

		public static T Instantiate<T, T1, T2>(this IContainer container, T1 argument1, T2 argument2)
		{
			return Instantiate<T>(container, CreateArguments(argument1, argument2));
		}

		public static T Instantiate<T, T1, T2, T3>(this IContainer container, T1 argument1, T2 argument2, T3 argument3)
		{
			return Instantiate<T>(container, CreateArguments(argument1, argument2, argument3));
		}

		public static T Instantiate<T>(this IContainer container, object[] arguments)
		{
			return Instantiate<T>(container, CreateArguments(arguments));
		}

		public static object Instantiate<T1>(this IContainer container, Type type, T1 argument1)
		{
			return Instantiate(container, type, CreateArguments(argument1));
		}

		public static object Instantiate<T1, T2>(this IContainer container, Type type, T1 argument1, T2 argument2)
		{
			return Instantiate(container, type, CreateArguments(argument1, argument2));
		}

		public static object Instantiate<T1, T2, T3>(this IContainer container, Type type, T1 argument1, T2 argument2, T3 argument3)
		{
			return Instantiate(container, type, CreateArguments(argument1, argument2, argument3));
		}

		public static object Instantiate(this IContainer container, Type type, object[] arguments)
		{
			return Instantiate(container, type, CreateArguments(arguments));
		}

		public static void Inject<T1>(this IContainer container, object instance, T1 argument1)
		{
			Inject(container, instance, CreateArguments(argument1));
		}

		public static void Inject<T1, T2>(this IContainer container, object instance, T1 argument1, T2 argument2)
		{
			Inject(container, instance, CreateArguments(argument1, argument2));
		}

		public static void Inject<T1, T2, T3>(this IContainer container, object instance, T1 argument1, T2 argument2, T3 argument3)
		{
			Inject(container, instance, CreateArguments(argument1, argument2, argument3));
		}

		public static void Inject(this IContainer container, object instance, object[] arguments)
		{
			Inject(container, instance, CreateArguments(arguments));
		}

		private static T Instantiate<T>(IContainer container, Argument[] arguments)
		{
			T instance;
			try
			{
				instance = container.Instantiate<T>(arguments);
			}
			finally
			{
				ArgumentsPool.Release(arguments);
			}
			return instance;
		}

		private static object Instantiate(IContainer container, Type type, Argument[] arguments)
		{
			object instance;
			try
			{
				instance = container.Instantiate(type, arguments);
			}
			finally
			{
				ArgumentsPool.Release(arguments);
			}
			return instance;
		}

		private static void Inject(IContainer container, object instance, Argument[] arguments)
		{
			try
			{
				container.Inject(instance, arguments);
			}
			finally
			{
				ArgumentsPool.Release(arguments);
			}
		}

		private static Argument[] CreateArguments<T1>(T1 argument1)
		{
			Argument[] arguments = ArgumentsPool.Pull(1) ?? new Argument[1];
			arguments[0] = new Argument(typeof(T1), argument1);
			return arguments;
		}

		private static Argument[] CreateArguments<T1, T2>(T1 argument1, T2 argument2)
		{
			Argument[] arguments = ArgumentsPool.Pull(2) ?? new Argument[2];
			arguments[0] = new Argument(typeof(T1), argument1);
			arguments[1] = new Argument(typeof(T2), argument2);
			return arguments;
		}

		private static Argument[] CreateArguments<T1, T2, T3>(T1 argument1, T2 argument2, T3 argument3)
		{
			Argument[] arguments = ArgumentsPool.Pull(3) ?? new Argument[3];
			arguments[0] = new Argument(typeof(T1), argument1);
			arguments[1] = new Argument(typeof(T2), argument2);
			arguments[2] = new Argument(typeof(T3), argument3);
			return arguments;
		}

		private static Argument[] CreateArguments(object[] values)
		{
			Argument[] arguments = ArgumentsPool.Pull(values.Length) ?? new Argument[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				object argument = values[i];
				arguments[i] = new Argument(argument.GetType(), argument);
			}
			return arguments;
		}
	}
}