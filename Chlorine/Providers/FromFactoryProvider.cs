using System;
using Chlorine.Factories;

namespace Chlorine.Providers
{
	internal sealed class FromFactoryProvider<T> : IProvider
			where T : class
	{
		private readonly IFactory<T> _factory;

		public FromFactoryProvider(IFactory<T> factory)
		{
			_factory = factory;
		}

		public object Provide()
		{
			return _factory.Create();
		}
	}

	internal sealed class FromFactoryTypeProvider<T> : IProvider, IDisposable
			where T : class
	{
		private readonly Type _factoryType;
		private readonly IContainer _container;

		private IFactory<T> _factory;

		public FromFactoryTypeProvider(Type factoryType, IContainer container)
		{
			_factoryType = factoryType;
			_container = container;
		}

		~FromFactoryTypeProvider()
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

		public object Provide()
		{
			if (_factory == null)
			{
				_factory = _container.Instantiate(_factoryType) as IFactory<T>;
			}
			return _factory.Create();
		}
	}

	internal sealed class FromFactoryMethodProvider<T> : IProvider
			where T : class
	{
		private readonly FactoryMethod<T> _factoryMethod;

		public FromFactoryMethodProvider(FactoryMethod<T> factoryMethod)
		{
			_factoryMethod = factoryMethod;
		}

		public object Provide()
		{
			return _factoryMethod.Invoke();
		}
	}
}