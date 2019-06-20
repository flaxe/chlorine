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

		public BindingTypeProvider<T> To<TInstance>() where TInstance : class, T
		{
			return new BindingTypeProvider<T>(_binder, null, new InstanceProvider<TInstance, T>(_container));
		}

		public BindingTypeProvider<T> FromFactory<TFactory>() where TFactory : class, IFactory<T>
		{
			return new BindingTypeProvider<T>(_binder, null, new FromFactoryProvider<TFactory, T>(_container));
		}

		public BindingTypeProvider<T> FromFactory(FactoryMethod<T> factoryMethod)
		{
			return new BindingTypeProvider<T>(_binder, null, new FromFactoryMethodProvider<T>(factoryMethod));
		}

		public void FromResolve<TResolve>(object id = null) where TResolve : class, T
		{
			_binder.Bind<T>(new FromContainerProvider<TResolve>(_container, id));
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
			_binder.Bind(new SingletonProvider<T>(new InstanceProvider<T, T>(_container)));
		}

		public void AsTransient()
		{
			_binder.Bind(new InstanceProvider<T, T>(_container));
		}
	}
}