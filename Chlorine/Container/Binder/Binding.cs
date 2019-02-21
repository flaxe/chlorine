namespace Chlorine
{
	public struct Binding<TContract>
			where TContract : class
	{
		private readonly Binder _binder;
		private readonly Container _container;

		internal Binding(Binder binder, Container container)
		{
			_binder = binder;
			_container = container;
		}

		public BindingWithId<TContract> WithId(object id)
		{
			return new BindingWithId<TContract>(_binder, _container, id);
		}

		public BindingWithProvider<TContract> To<TConcrete>() where TConcrete : class, TContract
		{
			return new BindingWithProvider<TContract>(_binder, null, new ConcreteProvider<TConcrete,TContract>(_container));
		}

		public BindingWithProvider<TContract> FromFactory<TFactory>() where TFactory : class, IFactory<TContract>
		{
			return new BindingWithProvider<TContract>(_binder, null, new FromFactoryProvider<TFactory, TContract>(_container));
		}

		public void ToInstance(TContract instance)
		{
			_binder.Bind(typeof(TContract), null, new InstanceProvider<TContract>(instance));
		}

		public void AsSingleton()
		{
			_binder.Bind(typeof(TContract), null, new SingletonProvider<TContract>(new ConcreteProvider<TContract, TContract>(_container)));
		}

		public void AsTransient()
		{
			_binder.Bind(typeof(TContract), null, new TransientProvider<TContract>(new ConcreteProvider<TContract, TContract>(_container)));
		}
	}
}