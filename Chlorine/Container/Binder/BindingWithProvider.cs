namespace Chlorine
{
	public struct BindingWithProvider<TContract>
			where TContract : class
	{
		private readonly Binder _binder;

		private readonly object _id;
		private readonly IProvider<TContract> _provider;

		internal BindingWithProvider(Binder binder, object id, IProvider<TContract> provider)
		{
			_binder = binder;
			_id = id;
			_provider = provider;
		}

		public void AsSingleton()
		{
			_binder.Bind(typeof(TContract), _id, new SingletonProvider<TContract>(_provider));
		}

		public void AsTransient()
		{
			_binder.Bind(typeof(TContract), _id, new TransientProvider<TContract>(_provider));
		}
	}
}