using System;

namespace Chlorine.Provider
{
	public sealed class FromFactoryProvider<TFactory, T> : IProvider<T>, IDisposable
			where TFactory : class, IFactory<T>
			where T : class
	{
		private readonly Container _container;
		private TFactory _factory;

		public FromFactoryProvider(Container container)
		{
			_container = container;
		}

		~FromFactoryProvider()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_factory != null)
			{
				if (_factory is IDisposable disposable)
				{
					disposable.Dispose();
				}
				_factory = null;
			}
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