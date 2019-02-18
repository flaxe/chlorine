namespace Chlorine
{
	internal class SingletonProvider<T> : IBindingProvider
			where T : class
	{
		private readonly IProvider<T> _provider;
		private T _instance;

		public SingletonProvider(IProvider<T> provider)
		{
			_provider = provider;
		}

		public object Provide()
		{
			return _instance ?? (_instance = _provider.Provide());
		}
	}
}