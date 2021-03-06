using System;

namespace Carbone.Providers
{
	public sealed class SingletonProvider : IProvider, IDisposable
	{
		private readonly IProvider _provider;
		private object? _instance;

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
				if (_instance is IDisposable instance)
				{
					instance.Dispose();
				}
				_instance = null;
			}
			if (_provider is IDisposable provider)
			{
				provider.Dispose();
			}
		}

		public object Provide()
		{
			return _instance ??= _provider.Provide();
		}
	}
}