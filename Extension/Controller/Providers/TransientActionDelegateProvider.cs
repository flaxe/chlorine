using System;

namespace Chlorine.Providers
{
	internal class TransientActionDelegateProvider<TAction> : IActionDelegateProvider<TAction>, IDisposable
			where TAction : struct
	{
		private readonly IProvider<IActionDelegate<TAction>> _provider;

		public TransientActionDelegateProvider(IProvider<IActionDelegate<TAction>> provider)
		{
			_provider = provider;
		}

		~TransientActionDelegateProvider()
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

		public IActionDelegate<TAction> Provide(ref TAction action)
		{
			return _provider.Provide();
		}

		public void Release(IActionDelegate<TAction> actionDelegate)
		{
		}
	}
}