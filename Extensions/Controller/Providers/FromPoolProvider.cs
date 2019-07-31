using System;
using Chlorine.Controller.Pools;
using Chlorine.Providers;

namespace Chlorine.Controller.Providers
{
	public sealed class FromPoolProvider<T> : IFromPoolProvider<T>, IDisposable
			where T : class
	{
		private readonly ProviderPool<T> _pool;

		public FromPoolProvider(IProvider<T> provider)
		{
			_pool = new ProviderPool<T>(provider);
		}

		~FromPoolProvider()
		{
			Dispose();
		}

		public void Dispose()
		{
			_pool.Dispose();
		}

		public T Provide()
		{
			return _pool.Pull();
		}

		object IProvider.Provide()
		{
			return Provide();
		}

		public void Release(T value, bool reset = true)
		{
			_pool.Release(value);
		}
	}
}