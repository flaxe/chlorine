namespace Chlorine
{
	internal class TransientProvider<T> : IProvider<T>
			where T : class
	{
		private readonly IProvider<T> _provider;

		public TransientProvider(IProvider<T> provider)
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
	}
}