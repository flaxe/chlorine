namespace Chlorine.Providers
{
	public sealed class InstanceProvider<T> : IProvider<T>
			where T : class
	{
		private readonly T _instance;

		public InstanceProvider(T instance)
		{
			_instance = instance;
		}

		public T Provide()
		{
			return _instance;
		}

		object IProvider.Provide()
		{
			return Provide();
		}
	}

	public sealed class InstanceProvider<TInstance, T> : IProvider<T>
			where TInstance : class, T
			where T : class
	{
		private readonly IContainer _container;

		public InstanceProvider(IContainer container)
		{
			_container = container;
		}

		public T Provide()
		{
			return _container.Instantiate<TInstance>();
		}

		object IProvider.Provide()
		{
			return Provide();
		}
	}
}