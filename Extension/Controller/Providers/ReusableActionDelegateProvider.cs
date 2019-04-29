using Chlorine.Pools;

namespace Chlorine.Providers
{
	internal class ReusableActionDelegateProvider<T> : IActionDelegateProvider<T> where T : class
	{
		private readonly ProviderPool<T> _pool;

		public ReusableActionDelegateProvider(IProvider<T> provider)
		{
			_pool = new ProviderPool<T>(provider);
		}

		public T Provide()
		{
			return _pool.Pull();
		}

		object IProvider.Provide()
		{
			return Provide();
		}

		public void Release(T actionDelegate)
		{
			_pool.Release(actionDelegate);
		}
	}
}