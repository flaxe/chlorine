namespace Chlorine
{
	internal class TransientProvider<T> : IBindingProvider
			where T : class
	{
		private readonly IProvider<T> _provider;

		public TransientProvider(IProvider<T> provider)
		{
			_provider = provider;
		}

		public object Provide()
		{
			return _provider.Provide();
		}
	}
}