namespace Chlorine
{
	public struct Binding<TContract>
			where TContract : class
	{
		private readonly Binder _binder;
		private readonly Container _container;

		private readonly object _id;

		internal Binding(Binder binder, Container container, object id)
		{
			_binder = binder;
			_container = container;
			_id = id;
		}

		public BindingScope<TContract> To<TConcrete>() where TConcrete : class, TContract
		{
			return new BindingScope<TContract>(_binder, _id, new ConcreteProvider<TConcrete, TContract>(_container));
		}

		public BindingScope<TContract> FromFactory<TFactory>() where TFactory : class, IFactory<TContract>
		{
			return new BindingScope<TContract>(_binder, _id, new FromFactoryProvider<TFactory, TContract>(_container));
		}

		public void ToInstance(TContract instance)
		{
			_binder.Bind(typeof(TContract), _id, new InstanceProvider<TContract>(instance));
		}

		public void AsSingleton()
		{
			_binder.Bind(typeof(TContract), _id, new SingletonProvider<TContract>(new ConcreteProvider<TContract, TContract>(_container)));
		}

		public void AsTransient()
		{
			_binder.Bind(typeof(TContract), _id, new TransientProvider<TContract>(new ConcreteProvider<TContract, TContract>(_container)));
		}
	}
}