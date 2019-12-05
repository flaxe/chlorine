using Chlorine.Factories;
using Chlorine.Providers;

namespace Chlorine.Bindings
{
	public struct BindingType<T>
			where T : class
	{
		private readonly Container _container;
		private readonly Binder _binder;

		internal BindingType(Container container, Binder binder)
		{
			_container = container;
			_binder = binder;
		}

		public BindingTypeId<T> WithId(object id)
		{
			return new BindingTypeId<T>(_container, _binder, id);
		}

		public BindingTypeProvider To<TInstance>() where TInstance : class, T
		{
			return new BindingTypeProvider(_binder, typeof(T), null, new TypeProvider(typeof(TInstance), _container));
		}

		public BindingTypeProvider FromFactory<TFactory>() where TFactory : class, IFactory<T>
		{
			return new BindingTypeProvider(_binder, typeof(T), null, new FromFactoryTypeProvider<T>(typeof(TFactory), _container));
		}

		public BindingTypeProvider FromFactory(IFactory<T> factory)
		{
			return new BindingTypeProvider(_binder, typeof(T), null, new FromFactoryProvider<T>(factory));
		}

		public BindingTypeProvider FromFactoryMethod(FactoryMethod<T> factoryMethod)
		{
			return new BindingTypeProvider(_binder, typeof(T), null, new FromFactoryMethodProvider<T>(factoryMethod));
		}

		public void FromResolve<TResolve>(object id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), new FromContainerProvider(_container, typeof(TResolve), id));
		}

		public void FromContainer(Container container)
		{
			_binder.Register(typeof(T), new FromContainerProvider(container, typeof(T)));
		}

		public void FromContainerResolve<TResolve>(Container container, object id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), new FromContainerProvider(container, typeof(TResolve), id));
		}

		public void ToInstance(T instance)
		{
			_binder.Register(typeof(T), new InstanceProvider(instance));
		}

		public void AsSingleton()
		{
			_binder.Register(typeof(T), new SingletonProvider(new TypeProvider(typeof(T), _container)));
		}

		public void AsTransient()
		{
			_binder.Register(typeof(T), new TypeProvider(typeof(T), _container));
		}
	}
}