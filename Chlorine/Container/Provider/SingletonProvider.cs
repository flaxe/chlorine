namespace Chlorine
{
	public class SingletonProvider<T> : IProvider<T>
			where T : class
	{
		private readonly IProvider<T> _provider;
		private T _instance;

		public SingletonProvider(IProvider<T> provider)
		{
			_provider = provider;
		}

		public T Provide()
		{
			return _instance ?? (_instance = _provider.Provide());
		}

		object IProvider.Provide()
		{
			return Provide();
		}
	}
}