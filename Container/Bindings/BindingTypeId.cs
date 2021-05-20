using Carbone.Providers;

namespace Carbone.Bindings
{
	public readonly struct BindingTypeId<T>
			where T : class
	{
		private readonly Container _container;
		private readonly Binder _binder;

		private readonly object _id;

		internal BindingTypeId(Container container, Binder binder, object id)
		{
			_container = container;
			_binder = binder;
			_id = id;
		}

		public BindingTypeConditional<T> When(BindingCondition condition)
		{
			return new BindingTypeConditional<T>(_container, _binder, _id, condition);
		}

		public BindingTypeConditional<T> WhenInjectInto<TContract>()
		{
			return new BindingTypeConditional<T>(_container, _binder, _id,
					context => context.SourceType == typeof(TContract));
		}

		public BindingTypeProvider To<TConcrete>() where TConcrete : class, T
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, null,
					new TypeProvider(typeof(TConcrete), _container));
		}

		public BindingTypeProvider FromFactory<TFactory>() where TFactory : class, IFactory<T>
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, null,
					new FromFactoryTypeProvider<T>(typeof(TFactory), _container));
		}

		public BindingTypeProvider FromFactory(IFactory<T> factory)
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, null,
					new FromFactoryProvider<T>(factory));
		}

		public BindingTypeProvider FromFactoryMethod(FactoryMethod<T> factoryMethod)
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, null,
					new FromFactoryMethodProvider<T>(factoryMethod));
		}

		public void FromResolve<TResolve>(object? id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), _id, null, new FromContainerProvider(_container, typeof(TResolve), id));
		}

		public void FromContainer(Container container)
		{
			_binder.Register(typeof(T), _id, null, new FromContainerProvider(container, typeof(T), _id));
		}

		public void FromContainerResolve<TResolve>(Container container, object? id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), _id, null, new FromContainerProvider(container, typeof(TResolve), id));
		}

		public void ToInstance(T instance)
		{
			_binder.Register(typeof(T), _id, null, new InstanceProvider(instance));
		}

		public void AsSingleton()
		{
			_binder.Register(typeof(T), _id, null, new SingletonProvider(new TypeProvider(typeof(T), _container)));
		}

		public void AsTransient()
		{
			_binder.Register(typeof(T), _id, null, new TypeProvider(typeof(T), _container));
		}
	}
}