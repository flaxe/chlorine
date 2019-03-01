using Chlorine.Provider;

namespace Chlorine.Binder
{
	public struct BindingType<T>
			where T : class
	{
		private readonly Container _container;
		private readonly ContainerBinder _binder;

		internal BindingType(Container container, ContainerBinder binder)
		{
			_container = container;
			_binder = binder;
		}

		public BindingTypeId<T> WithId(object id)
		{
			return new BindingTypeId<T>(_container, _binder, id);
		}

		public BindingTypeProvider<T> To<TConcrete>() where TConcrete : class, T
		{
			return new BindingTypeProvider<T>(_binder, null, new ConcreteProvider<TConcrete, T>(_container));
		}

		public BindingTypeProvider<T> FromFactory<TFactory>() where TFactory : class, IFactory<T>
		{
			return new BindingTypeProvider<T>(_binder, null, new FromFactoryProvider<TFactory, T>(_container));
		}

		public void FromContainer(Container container)
		{
			_binder.Bind(new FromContainerProvider<T>(container));
		}

		public void ToInstance(T instance)
		{
			_binder.Bind(new InstanceProvider<T>(instance));
		}

		public void AsSingleton()
		{
			_binder.Bind(new SingletonProvider<T>(new ConcreteProvider<T, T>(_container)));
		}

		public void AsTransient()
		{
			_binder.Bind(new TransientProvider<T>(new ConcreteProvider<T, T>(_container)));
		}
	}
}