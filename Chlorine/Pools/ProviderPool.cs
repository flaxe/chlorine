using System;
using Chlorine.Providers;

namespace Chlorine.Pools
{
	public sealed class ProviderPool<T> : IDisposable where T : class
	{
		private static readonly Type Type = typeof(T);

		private readonly IProvider<T> _provider;

		public ProviderPool(IProvider<T> provider)
		{
			_provider = provider;
		}

		~ProviderPool()
		{
			Dispose();
		}

		public void Dispose()
		{
			Clear();
		}

		public void Clear()
		{
			SharedPool.Clear(Type);
		}

		public T Pull()
		{
			T value = SharedPool.Pull<T>();
			if (value != null)
			{
				return value;
			}
			return _provider.Provide();
		}

		public void Release(T value, bool reset = true)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			SharedPool.UnsafeRelease(Type, value, reset);
		}
	}
}