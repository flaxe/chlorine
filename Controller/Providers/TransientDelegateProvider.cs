using System;

namespace Carbone.Providers
{
	internal sealed class TransientDelegateProvider : IDelegateProvider, IDisposable
	{
		private readonly IProvider _provider;

		public TransientDelegateProvider(IProvider provider)
		{
			_provider = provider;
		}

		~TransientDelegateProvider()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_provider is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		public object Provide()
		{
			return _provider.Provide();
		}

		public void Release(object value)
		{
		}
	}
}