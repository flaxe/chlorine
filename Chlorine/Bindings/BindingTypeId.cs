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

		public BindingTypeProvider<T> To<TInstance>() where TInstance : class, T
		{
			return new BindingTypeProvider<T>(_binder, _id, new InstanceProvider<TInstance, T>(_container));
		}

		public BindingTypeProvider<T> FromFactory<TFactory>() where TFactory : class, IFactory<T>
		{
			return new BindingTypeProvider<T>(_binder, _id, new FromFactoryProvider<TFactory, T>(_container));
		}

		public BindingTypeProvider<T> FromFactory(IFactory<T> factory)
		{
			return new BindingTypeProvider<T>(_binder, _id, new FromFactoryProvider<T>(factory));
		}

		public BindingTypeProvider<T> FromFactoryMethod(FactoryMethod<T> factoryMethod)
		{
			return new BindingTypeProvider<T>(_binder, _id, new FromFactoryMethodProvider<T>(factoryMethod));
		}

		public void FromResolve<TResolve>(object id = null) where TResolve : class, T
		{
			_binder.Bind<T>(_id, new FromContainerProvider<TResolve>(_container, id));
		}

		public void FromContainer(Container container)
		{
			_binder.Bind(_id, new FromContainerProvider<T>(container, _id));
		}

		public void FromContainerResolve<TResolve>(Container container, object id = null) where TResolve : class, T
		{
			_binder.Bind<T>(_id, new FromContainerProvider<TResolve>(container, id));
		}

		public void ToInstance(T instance)
		{
			_binder.Bind(_id, new InstanceProvider<T>(instance));
		}

		public void AsSingleton()
		{
			_binder.Bind(_id, new SingletonProvider<T>(new InstanceProvider<T, T>(_container)));
		}

		public void AsTransient()
		{
			_binder.Bind(_id, new InstanceProvider<T, T>(_container));
		}
	}
}