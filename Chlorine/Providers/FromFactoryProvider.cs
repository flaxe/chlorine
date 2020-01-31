using System;
using Chlorine.Exceptions;
using Chlorine.Factories;

namespace Chlorine.Providers
{
	public sealed class FromFactoryProvider<T> : IProvider
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

	public sealed class FromFactoryTypeProvider<T> : IProvider, IDisposable
			where T : class
	{
		private readonly Type _factoryType;
		private readonly IContainer _container;

		private IFactory<T> _factory;

		public FromFactoryTypeProvider(Type factoryType, IContainer container)
		{
			if (factoryType.IsInterface || factoryType.IsAbstract)
			{
				throw new InvalidArgumentException(InvalidArgumentErrorCode.InvalidType,
						$"Type '{factoryType.Name}' is abstract.");
			}
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

	public sealed class FromFactoryMethodProvider<T> : IProvider
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