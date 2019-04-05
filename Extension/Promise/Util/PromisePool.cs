using System;
using System.Collections.Generic;

namespace Chlorine
{
	public sealed class PromisePool
	{
		private Dictionary<Type, Stack<object>> _stackByType;

		public Promise Get()
		{
			return TryPull(out Promise promise) ? promise : new Promise();
		}

		public Promise<TResult> Get<TResult>()
		{
			return TryPull(out Promise<TResult> promise) ? promise : new Promise<TResult>();
		}

		public void Release(IPromise promise)
		{
			if (promise == null)
			{
				throw new ArgumentNullException(nameof(promise));
			}
			if (promise is IPoolable poolable)
			{
				poolable.Reset();
			}
			Type type = promise.GetType();
			if (_stackByType != null && _stackByType.TryGetValue(type, out Stack<object> stack))
			{
				stack.Push(promise);
			}
			else
			{
				stack = new Stack<object>();
				stack.Push(promise);
				if (_stackByType == null)
				{
					_stackByType = new Dictionary<Type, Stack<object>> {{type, stack}};
				}
				else
				{
					_stackByType.Add(type, stack);
				}
			}
		}

		private bool TryPull<TPromise>(out TPromise promise) where TPromise : class, IPromise
		{
			if (_stackByType != null && _stackByType.TryGetValue(typeof(TPromise), out Stack<object> stack) && stack.Count > 0)
			{
				promise = stack.Pop() as TPromise;
				return true;
			}
			promise = default;
			return false;
		}
	}
}