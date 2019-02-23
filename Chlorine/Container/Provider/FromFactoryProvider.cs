namespace Chlorine
{
	internal class FromFactoryProvider<TFactory, T> : IProvider<T>
			where TFactory : class, IFactory<T>
			where T : class
	{
		private readonly Container _container;
		private TFactory _factory;

		public FromFactoryProvider(Container container)
		{
			_container = container;
		}

		public T Provide()
		{
			if (_factory == null)
			{
				_factory = _container.Instantiate<TFactory>();
			}
			return _factory.Create();
		}

		object IProvider.Provide()
		{
			return Provide();
		}
	}
}