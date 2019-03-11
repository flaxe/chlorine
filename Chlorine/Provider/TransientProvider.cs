using System;

namespace Chlorine.Provider
{
	public sealed class TransientProvider<T> : IProvider<T>, IDisposable
			where T : class
	{
		private readonly IProvider<T> _provider;

		public TransientProvider(IProvider<T> provider)
		{
			_provider = provider;
		}

		~TransientProvider()
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