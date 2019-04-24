using System;
using System.Collections.Generic;

namespace Chlorine
{
	public sealed class FuturePool
	{
		private Dictionary<Type, Stack<object>> _stackByType;

		public Future Get(IPromise promise)
		{
			if (TryPull(out Future future))
			{
				future.Init(promise);
				return future;
			}
			return new Future(promise);
		}

		public Future<TResult> Get<TResult>(IPromise<TResult> promise)
		{
			if (TryPull(out Future<TResult> future))
			{
				future.Init(promise);
				return future;
			}
			return new Future<TResult>(promise);
		}

		public IFuture GetTrue()
		{
			if (TryPull(out TrueFuture future))
			{
				return future;
			}
			return new TrueFuture();
		}

		public IFuture<TResult> GetTrue<TResult>(TResult result)
		{
			if (TryPull(out TrueFuture<TResult> future))
			{
				future.Resolve(result);
				return future;
			}
			return new TrueFuture<TResult>();
		}

		public IFuture GetFalse(Error error)
		{
			if (TryPull(out FalseFuture future))
			{
				future.Reject(error);
				return future;
			}
			return new FalseFuture(error);
		}

		public IFuture<TResult> GetFalse<TResult>(Error error)
		{
			if (TryPull(out FalseFuture<TResult> future))
			{
				future.Reject(error);
				return future;
			}
			return new FalseFuture<TResult>(error);
		}

		public void Release(IFuture future)
		{
			if (future == null)
			{
				throw new ArgumentNullException(nameof(future));
			}
			if (future is IPoolable poolable)
			{
				poolable.Reset();
			}
			Type type = future.GetType();
			if (_stackByType != null && _stackByType.TryGetValue(type, out Stack<object> stack))
			{
				stack.Push(future);
			}
			else
			{
				stack = new Stack<object>();
				stack.Push(future);
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

		private bool TryPull<TFuture>(out TFuture future) where TFuture : class, IFuture
		{
			if (_stackByType != null && _stackByType.TryGetValue(typeof(TFuture), out Stack<object> stack) && stack.Count > 0)
			{
				future = stack.Pop() as TFuture;
				return true;
			}
			future = default;
			return false;
		}
	}
}