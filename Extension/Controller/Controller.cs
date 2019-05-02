using System;
using Chlorine.Supervisors;
using Chlorine.Bindings;
using Chlorine.Exceptions;
using Chlorine.Pools;

namespace Chlorine
{
	internal sealed class Controller : IController, IDisposable
	{
		private readonly ControllerBinder _binder;

		public Controller(ControllerBinder binder)
		{
			_binder = binder;
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
			if (!_binder.TryResolveSupervisor(out IActionSupervisor<TAction> actionSupervisor))
			{
				throw new ControllerException(ControllerErrorCode.ActionNotRegistered,
						$"Unable to perform {typeof(TAction).Name}. Action not registered.");
			}
			Expected<IPromise> expectedPromise = actionSupervisor.Perform(ref action);
			if (expectedPromise.TryGetValue(out IPromise promise))
			{
				return FuturePool.Pull(promise);
			}
			throw (ControllerException)expectedPromise.Error;
		}

		public IFuture<TResult> Perform<TAction, TResult>(TAction action) where TAction : struct
		{
			if (!_binder.TryResolveSupervisor(out IActionSupervisor<TAction> actionSupervisor))
			{
				throw new ControllerException(ControllerErrorCode.ActionNotRegistered,
						$"Unable to perform {typeof(TAction).Name}. Action not registered.");
			}
			if (actionSupervisor is IActionSupervisor<TAction, TResult> actionResultSupervisor)
			{
				Expected<IPromise<TResult>> expectedPromise = actionResultSupervisor.Perform(ref action);
				if (expectedPromise.TryGetValue(out IPromise<TResult> promise))
				{
					return FuturePool<TResult>.Pull(promise);
				}
				throw (ControllerException)expectedPromise.Error;
			}
			throw new ControllerException(ControllerErrorCode.ActionDoesNotReturnResult,
					$"Unable to perform {typeof(TAction).Name}. Action does not return {typeof(TResult).Name}.");
		}

		public IFuture TryPerform<TAction>(TAction action) where TAction : struct
		{
			if (!_binder.TryResolveSupervisor(out IActionSupervisor<TAction> actionSupervisor))
			{
				return FuturePool.PullRejected(new Error((int)ControllerErrorCode.ActionNotRegistered,
						$"Unable to perform {typeof(TAction).Name}. Action not registered."));
			}
			Expected<IPromise> expectedPromise = actionSupervisor.Perform(ref action);
			if (expectedPromise.TryGetValue(out IPromise promise))
			{
				return FuturePool.Pull(promise);
			}
			return FuturePool.PullRejected(expectedPromise.Error);
		}

		public IFuture<TResult> TryPerform<TAction, TResult>(TAction action) where TAction : struct
		{
			if (!_binder.TryResolveSupervisor(out IActionSupervisor<TAction> actionSupervisor))
			{
				return FuturePool<TResult>.PullRejected(new Error((int)ControllerErrorCode.ActionNotRegistered,
						$"Unable to perform {typeof(TAction).Name}. Action not registered."));
			}
			if (actionSupervisor is IActionSupervisor<TAction, TResult> actionResultSupervisor)
			{
				Expected<IPromise<TResult>> expectedPromise = actionResultSupervisor.Perform(ref action);
				if (expectedPromise.TryGetValue(out IPromise<TResult> promise))
				{
					return FuturePool<TResult>.Pull(promise);
				}
				return FuturePool<TResult>.PullRejected(expectedPromise.Error);
			}
			return FuturePool<TResult>.PullRejected(new Error((int)ControllerErrorCode.ActionDoesNotReturnResult,
					$"Unable to perform {typeof(TAction).Name}. Action does not return {typeof(TResult).Name}."));
		}
	}
}