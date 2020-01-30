using Chlorine.Factories;
using Chlorine.Providers;

namespace Chlorine.Bindings
{
	public struct BindingTypeConditional<T>
			where T : class
	{
		private readonly Container _container;
		private readonly Binder _binder;

		private readonly object _id;
		private readonly BindingCondition _condition;

		internal BindingTypeConditional(Container container, Binder binder, object id, BindingCondition condition)
		{
			_container = container;
			_binder = binder;
			_id = id;
			_condition = condition;
		}

		public BindingTypeProvider To<TConcrete>() where TConcrete : class, T
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, _condition,
					new TypeProvider(typeof(TConcrete), _container));
		}

		public BindingTypeProvider FromFactory<TFactory>() where TFactory : class, IFactory<T>
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, _condition,
					new FromFactoryTypeProvider<T>(typeof(TFactory), _container));
		}

		public BindingTypeProvider FromFactory(IFactory<T> factory)
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, _condition,
					new FromFactoryProvider<T>(factory));
		}

		public BindingTypeProvider FromFactoryMethod(FactoryMethod<T> factoryMethod)
		{
			return new BindingTypeProvider(_binder, typeof(T), _id, _condition,
					new FromFactoryMethodProvider<T>(factoryMethod));
		}

		public void FromResolve<TResolve>(object id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), _id, _condition, new FromContainerProvider(_container, typeof(TResolve), id));
		}

		public void FromContainer(Container container)
		{
			_binder.Register(typeof(T), _id, _condition, new FromContainerProvider(container, typeof(T), _id));
		}

		public void FromContainerResolve<TResolve>(Container container, object id = null) where TResolve : class, T
		{
			_binder.Register(typeof(T), _id, _condition,
					new FromContainerProvider(container, typeof(TResolve), id));
		}

		public void ToInstance(T instance)
		{
			_binder.Register(typeof(T), _id, _condition, new InstanceProvider(instance));
		}

		public void AsSingleton()
		{
			_binder.Register(typeof(T), _id, _condition, new SingletonProvider(new TypeProvider(typeof(T), _container)));
		}

		public void AsTransient()
		{
			_binder.Register(typeof(T), _id, _condition, new TypeProvider(typeof(T), _container));
		}
	}
}