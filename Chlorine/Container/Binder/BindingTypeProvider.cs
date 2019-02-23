namespace Chlorine
{
	public struct BindingTypeProvider<T>
			where T : class
	{
		private readonly Binder _binder;

		private readonly object _id;
		private readonly IProvider<T> _provider;

		internal BindingTypeProvider(Binder binder, object id, IProvider<T> provider)
		{
			_binder = binder;
			_id = id;
			_provider = provider;
		}

		public void AsSingleton()
		{
			_binder.Bind(_id, new SingletonProvider<T>(_provider));
		}

		public void AsTransient()
		{
			_binder.Bind(_id, new TransientProvider<T>(_provider));
		}
	}
}