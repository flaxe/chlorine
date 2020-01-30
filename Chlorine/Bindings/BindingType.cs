using Chlorine.Factories;
using Chlorine.Injection;
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

		public BindingTypeConditional<T> When(BindingCondition condition)
		{
			return new BindingTypeConditional<T>(_container, _binder, null, condition);
		}

		public BindingTypeConditional<T> WhenInjectInto<TContract>()
		{
			return new BindingTypeConditional<T>(_container, _binder, null,
					(in InjectContext context) => context.SourceType == typeof(TContract));
		}

		public BindingTypeId<T> WithId(object id)
		{
			return new BindingTypeId<T>(_container, _binder, id);
		}

		public BindingTypeProvider To<TConcrete>() where TConcrete : class, T
		{
			return new BindingTypeProvider(_binder, typeof(T), null, null,
					new TypeProvider(typeof(TConcrete), _container));
		}

		public BindingTypeProvider FromFactory<TFactory>() where TFactory : class, IFactory<T>
		{
			return new BindingTypeProvider(_binder, typeof(T), null, null,
					new FromFactoryTypeProvider<T>(typeof(TFactory), _container));
		}

		public BindingTypeProvider FromFactory(IFactory<T> factory)
		{
			return new BindingTypeProvider(_binder, typeof(T), null, null,
					new FromFactoryProvider<T>(factory));
		}

		public BindingTypeProvider FromFactoryMethod(FactoryMethod<T> factoryMethod)
		{
			return new BindingTypeProvider(_binder, typeof(T), null, null,
					new FromFactoryMethodProvider<T>(factoryMethod));
		}

		public void FromResolve<TResolve>(object id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), null, null, new FromContainerProvider(_container, typeof(TResolve), id));
		}

		public void FromContainer(Container container)
		{
			_binder.Register(typeof(T), null, null, new FromContainerProvider(container, typeof(T)));
		}

		public void FromContainerResolve<TResolve>(Container container, object id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), null, null, new FromContainerProvider(container, typeof(TResolve), id));
		}

		public void ToInstance(T instance)
		{
			_binder.Register(typeof(T), null, null, new InstanceProvider(instance));
		}

		public void AsSingleton()
		{
			_binder.Register(typeof(T), null, null, new SingletonProvider(new TypeProvider(typeof(T), _container)));
		}

		public void AsTransient()
		{
			_binder.Register(typeof(T), null, null, new TypeProvider(typeof(T), _container));
		}
	}
}