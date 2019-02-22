namespace Chlorine
{
	public struct BindingTypeId<TContract>
			where TContract : class
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

		public BindingTypeProvider<TContract> To<TConcrete>() where TConcrete : class, TContract
		{
			return new BindingTypeProvider<TContract>(_binder, _id, new ConcreteProvider<TConcrete, TContract>(_container));
		}

		public BindingTypeProvider<TContract> FromFactory<TFactory>() where TFactory : class, IFactory<TContract>
		{
			return new BindingTypeProvider<TContract>(_binder, _id, new FromFactoryProvider<TFactory, TContract>(_container));
		}

		public void FromSubContainer<TInstaller>() where TInstaller : class, IInstaller
		{
			_binder.Bind<TContract>(_id, new FromSubContainerProvider<TInstaller, TContract>(_container, _id));
		}

		public void ToInstance(TContract instance)
		{
			_binder.Bind<TContract>(_id, new InstanceProvider<TContract>(instance));
		}

		public void AsSingleton()
		{
			_binder.Bind<TContract>(_id, new SingletonProvider<TContract>(new ConcreteProvider<TContract, TContract>(_container)));
		}

		public void AsTransient()
		{
			_binder.Bind<TContract>(_id, new TransientProvider<TContract>(new ConcreteProvider<TContract, TContract>(_container)));
		}
	}
}