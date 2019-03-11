using System;

namespace Chlorine.Provider
{
	public sealed class SingletonProvider<T> : IProvider<T>, IDisposable
			where T : class
	{
		private readonly IProvider<T> _provider;
		private T _instance;

		public SingletonProvider(IProvider<T> provider)
		{
			_provider = provider;
		}

		~SingletonProvider()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_instance != null)
			{
				if (_instance is IDisposable disposableInstance)
				{
					disposableInstance.Dispose();
				}
				_instance = null;
			}
			if (_provider is IDisposable disposableProvider)
			{
				disposableProvider.Dispose();
			}
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