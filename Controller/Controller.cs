using System;
using Carbone.Bindings;
using Carbone.Exceptions;
using Carbone.Futures;
using Carbone.Supervisors;

namespace Carbone
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

		public IFuture Perform<TAction>(in TAction action) where TAction : struct
		{
			if (_binder.TryResolveSupervisor(typeof(TAction), out object? value) &&
					value is IActionSupervisor<TAction> actionSupervisor)
			{
				Expected<IFuture> expected = actionSupervisor.Perform(in action);
				if (expected.TryGetValue(out IFuture future))
				{
					return future;
				}
				throw expected.Error.ToException();
			}
			throw new ControllerException(ControllerErrorCode.ActionNotRegistered,
					$"Unable to perform \"{typeof(TAction).Name}\". Action not registered.");
		}

		public IFuture<TResult> Perform<TAction, TResult>(in TAction action) where TAction : struct
		{
			if (_binder.TryResolveSupervisor(typeof(TAction), out object? value))
			{
				if (value is IActionSupervisor<TAction, TResult> actionSupervisor)
				{
					Expected<IFuture<TResult>> expected = actionSupervisor.Perform(in action);
					if (expected.TryGetValue(out IFuture<TResult> future))
					{
						return future;
					}
					throw expected.Error.ToException();
				}
				throw new ControllerException(ControllerErrorCode.ActionHasNoResult,
						$"Unable to perform \"{typeof(TAction).Name}\". Action has no result \"{typeof(TResult).Name}\".");
			}
			throw new ControllerException(ControllerErrorCode.ActionNotRegistered,
					$"Unable to perform \"{typeof(TAction).Name}\". Action not registered.");
		}

		public IFuture TryPerform<TAction>(in TAction action) where TAction : struct
		{
			if (_binder.TryResolveSupervisor(typeof(TAction), out object? value) &&
					value is IActionSupervisor<TAction> actionSupervisor)
			{
				Expected<IFuture> expected = actionSupervisor.Perform(in action);
				if (expected.TryGetValue(out IFuture future))
				{
					return future;
				}
				return FuturePool.PullRejected(expected.Error);
			}
			return FuturePool.PullRejected(new Error((int)ControllerErrorCode.ActionNotRegistered,
					$"Unable to perform \"{typeof(TAction).Name}\". Action not registered."));
		}

		public IFuture<TResult> TryPerform<TAction, TResult>(in TAction action) where TAction : struct
		{
			if (_binder.TryResolveSupervisor(typeof(TAction), out object? value))
			{
				if (value is IActionSupervisor<TAction, TResult> actionSupervisor)
				{
					Expected<IFuture<TResult>> expected = actionSupervisor.Perform(in action);
					if (expected.TryGetValue(out IFuture<TResult> future))
					{
						return future;
					}
					return FuturePool.PullRejected<TResult>(expected.Error);
				}
				return FuturePool.PullRejected<TResult>(new Error((int)ControllerErrorCode.ActionHasNoResult,
						$"Unable to perform \"{typeof(TAction).Name}\". Action has no result \"{typeof(TResult).Name}\"."));
			}
			return FuturePool.PullRejected<TResult>(new Error((int)ControllerErrorCode.ActionNotRegistered,
					$"Unable to perform \"{typeof(TAction).Name}\". Action not registered."));
		}
	}
}