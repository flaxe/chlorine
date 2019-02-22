namespace Chlorine
{
	public struct BindingType<TContract>
			where TContract : class
	{
		private readonly Container _container;
		private readonly Binder _binder;

		internal BindingType(Container container, Binder binder)
		{
			_container = container;
			_binder = binder;
		}

		public BindingTypeId<TContract> WithId(object id)
		{
			return new BindingTypeId<TContract>(_container, _binder, id);
		}

		public BindingTypeProvider<TContract> To<TConcrete>()where TConcrete : class, TContract
		{
			return new BindingTypeProvider<TContract>(_binder, null, new ConcreteProvider<TConcrete,TContract>(_container));
		}

		public BindingTypeProvider<TContract> FromFactory<TFactory>() where TFactory : class, IFactory<TContract>
		{
			return new BindingTypeProvider<TContract>(_binder, null, new FromFactoryProvider<TFactory, TContract>(_container));
		}

		public void FromSubContainer<TInstaller>() where TInstaller : class, IInstaller
		{
			_binder.Bind<TContract>(new FromSubContainerProvider<TInstaller, TContract>(_container));
		}

		public void ToInstance(TContract instance)
		{
			_binder.Bind<TContract>(new InstanceProvider<TContract>(instance));
		}

		public void AsSingleton()
		{
			_binder.Bind<TContract>(new SingletonProvider<TContract>(new ConcreteProvider<TContract, TContract>(_container)));
		}

		public void AsTransient()
		{
			_binder.Bind<TContract>(new TransientProvider<TContract>(new ConcreteProvider<TContract, TContract>(_container)));
		}
	}
}