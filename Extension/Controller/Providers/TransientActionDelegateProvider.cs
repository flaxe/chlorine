namespace Chlorine.Providers
{
	internal class TransientActionDelegateProvider<T> : IActionDelegateProvider<T> where T : class
	{
		private readonly IProvider<T> _provider;

		public TransientActionDelegateProvider(IProvider<T> provider)
		{
			_provider = provider;
		}

		public T Provide()
		{
			return _provider.Provide();
		}

		object IProvider.Provide()
		{
			return Provide();
		}

		public void Release(T actionDelegate)
		{
		}
	}
}