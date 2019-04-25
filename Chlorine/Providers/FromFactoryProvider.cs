using System;

namespace Chlorine.Providers
{
	public sealed class FromFactoryProvider<TFactory, T> : IProvider<T>, IDisposable
			where TFactory : class, IFactory<T>
			where T : class
	{
		private readonly IContainer _container;
		private TFactory _factory;

		public FromFactoryProvider(IContainer container)
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

	public sealed class FromFactoryMethodProvider<T> : IProvider<T>
			where T : class
	{
		private readonly FactoryMethod<T> _factoryMethod;

		public FromFactoryMethodProvider(FactoryMethod<T> factoryMethod)
		{
			_factoryMethod = factoryMethod;
		}

		public T Provide()
		{
			return _factoryMethod.Invoke();
		}

		object IProvider.Provide()
		{
			return Provide();
		}
	}
}