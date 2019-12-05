using Chlorine.Factories;
using Chlorine.Providers;

namespace Chlorine.Bindings
{
	public struct BindingTypeId<T>
			where T : class
	{
		private readonly Binder _binder;
		private readonly Container _container;

		private readonly object _id;

		internal BindingTypeId(Container container, Binder binder, object id)
		{
			_container = container;
			_binder = binder;
			_id = id;
		}

		public BindingTypeProvider To<TInstance>() where TInstance : class, T
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, new TypeProvider(typeof(TInstance), _container));
		}

		public BindingTypeProvider FromFactory<TFactory>() where TFactory : class, IFactory<T>
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, new FromFactoryTypeProvider<T>(typeof(TFactory), _container));
		}

		public BindingTypeProvider FromFactory(IFactory<T> factory)
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, new FromFactoryProvider<T>(factory));
		}

		public BindingTypeProvider FromFactoryMethod(FactoryMethod<T> factoryMethod)
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, new FromFactoryMethodProvider<T>(factoryMethod));
		}

		public void FromResolve<TResolve>(object id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), _id, new FromContainerProvider(_container, typeof(TResolve), id));
		}

		public void FromContainer(Container container)
		{
			_binder.Register(typeof(T), _id, new FromContainerProvider(container, typeof(T), _id));
		}

		public void FromContainerResolve<TResolve>(Container container, object id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), _id, new FromContainerProvider(container, typeof(TResolve), id));
		}

		public void ToInstance(T instance)
		{
			_binder.Register(typeof(T), _id, new InstanceProvider(instance));
		}

		public void AsSingleton()
		{
			_binder.Register(typeof(T), _id, new SingletonProvider(new TypeProvider(typeof(T), _container)));
		}

		public void AsTransient()
		{
			_binder.Register(typeof(T), _id, new TypeProvider(typeof(T), _container));
		}
	}
}