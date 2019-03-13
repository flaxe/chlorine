using Chlorine.Provider;

namespace Chlorine.Binder
{
	public struct BindingTypeProvider<T>
			where T : class
	{
		private readonly ContainerBinder _binder;

		private readonly object _id;
		private readonly IProvider<T> _provider;

		internal BindingTypeProvider(ContainerBinder binder, object id, IProvider<T> provider)
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
			_binder.Bind(_id, _provider);
		}
	}
}