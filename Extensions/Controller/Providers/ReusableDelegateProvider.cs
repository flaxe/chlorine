using System;
using System.Collections.Generic;
using Chlorine.Providers;

namespace Chlorine.Controller.Providers
{
	internal sealed class ReusableDelegateProvider : IDelegateProvider, IDisposable
	{
		private readonly IProvider _provider;
		private Stack<object> _stack;

		public ReusableDelegateProvider(IProvider provider)
		{
			_provider = provider;
		}

		~ReusableDelegateProvider()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_provider is IDisposable disposableProvider)
			{
				disposableProvider.Dispose();
			}
			if (_stack != null && _stack.Count > 0)
			{
				foreach (object instance in _stack)
				{
					if (instance is IDisposable disposableInstance)
					{
						disposableInstance.Dispose();
					}
				}
				_stack.Clear();
			}
		}

		public object Provide()
		{
			if (_stack != null && _stack.Count > 0)
			{
				return _stack.Pop();
			}
			return _provider.Provide();
		}

		public void Release(object instance)
		{
			if (instance is IPoolable poolable)
			{
				poolable.Reset();
			}
			if (_stack == null)
			{
				_stack = new Stack<object>();
			}
			_stack.Push(instance);
		}
	}
}