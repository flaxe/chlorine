using System;
using System.Collections.Generic;
using Chlorine.Controller.Providers;

namespace Chlorine.Controller.Pools
{
	public sealed class ProviderPool<T> : IDisposable where T : class
	{
		private static readonly Stack<T> Stack = new Stack<T>();

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
			Stack.Clear();
		}

		public T Pull()
		{
			if (Stack.Count > 0)
			{
				return Stack.Pop();
			}
			return _provider.Provide();
		}

		public void Release(T value, bool reset = true)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			if (reset && value is IPoolable poolable)
			{
				poolable.Reset();
			}
			Stack.Push(value);
		}
	}
}