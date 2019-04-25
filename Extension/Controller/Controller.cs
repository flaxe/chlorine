using System;
using Chlorine.Action;
using Chlorine.Bindings;

namespace Chlorine
{
	internal sealed class Controller : IController, IDisposable
	{
		private readonly ControllerBinder _binder;
		private readonly FuturePool _futurePool;

		public Controller(ControllerBinder binder, FuturePool futurePool)
		{
			_binder = binder;
			_futurePool = futurePool;
		}

		~Controller()
		{
			Dispose();
		}

		public void Dispose()
		{
			_binder.Dispose();
		}

		public IFuture Perform<TAction>(TAction action) where TAction : struct
		{
			if (!_binder.TryResolveActionSupervisor(out IActionSupervisor<TAction> actionSupervisor))
			{
				throw new ControllerException($"Unable to perform {typeof(TAction).Name}. Action not registered.");
			}
			Expected<IPromise> expectedPromise = actionSupervisor.TryPerform(ref action);
			if (expectedPromise.TryGetValue(out IPromise promise))
			{
				return _futurePool.Get(promise);
			}
			throw (Exception)expectedPromise.Error;
		}

		public IFuture<TResult> Perform<TAction, TResult>(TAction action) where TAction : struct
		{
			if (!_binder.TryResolveActionSupervisor(out IActionSupervisor<TAction> actionSupervisor))
			{
				throw new ControllerException($"Unable to perform {typeof(TAction).Name}. Action not registered.");
			}
			if (actionSupervisor is IActionSupervisor<TAction, TResult> actionResultSupervisor)
			{
				Expected<IPromise<TResult>> expectedPromise = actionResultSupervisor.TryPerform(ref action);
				if (expectedPromise.TryGetValue(out IPromise<TResult> promise))
				{
					return _futurePool.Get(promise);
				}
				throw (Exception)expectedPromise.Error;
			}
			throw new ControllerException($"Unable to perform {typeof(TAction).Name}. Action does not return {typeof(TResult).Name}.");
		}

		public IFuture TryPerform<TAction>(TAction action) where TAction : struct
		{
			if (!_binder.TryResolveActionSupervisor(out IActionSupervisor<TAction> actionSupervisor))
			{
				return _futurePool.GetFalse(new Error($"Unable to perform {typeof(TAction).Name}. Action not registered."));
			}
			Expected<IPromise> expectedPromise = actionSupervisor.TryPerform(ref action);
			if (expectedPromise.TryGetValue(out IPromise promise))
			{
				return _futurePool.Get(promise);
			}
			return _futurePool.GetFalse(expectedPromise.Error);
		}

		public IFuture<TResult> TryPerform<TAction, TResult>(TAction action) where TAction : struct
		{
			if (!_binder.TryResolveActionSupervisor(out IActionSupervisor<TAction> actionSupervisor))
			{
				return _futurePool.GetFalse<TResult>(new Error($"Unable to perform {typeof(TAction).Name}. Action not registered."));
			}
			if (actionSupervisor is IActionSupervisor<TAction, TResult> actionResultSupervisor)
			{
				Expected<IPromise<TResult>> expectedPromise = actionResultSupervisor.TryPerform(ref action);
				if (expectedPromise.TryGetValue(out IPromise<TResult> promise))
				{
					return _futurePool.Get(promise);
				}
				return _futurePool.GetFalse<TResult>(expectedPromise.Error);
			}
			return _futurePool.GetFalse<TResult>(new Error($"Unable to perform {typeof(TAction).Name}. Action does not return {typeof(TResult).Name}."));
		}
	}
}