using System;

namespace Chlorine.Providers
{
	public sealed class SingletonProvider : IProvider, IDisposable
	{
		private readonly IProvider _provider;
		private object _instance;

		public SingletonProvider(IProvider provider)
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

		public object Provide()
		{
			return _instance ?? (_instance = _provider.Provide());
		}
	}
}