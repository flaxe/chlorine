using System;
using System.Collections.Generic;
using Chlorine.Controller.Bindings;
using Chlorine.Controller.Exceptions;
using Chlorine.Controller.Execution;
using Chlorine.Futures;
using Chlorine.Pools;

namespace Chlorine.Controller.Supervisors.Internal
{
	internal abstract class AbstractExecutionSupervisor : AbstractSupervisor
	{
		private Dictionary<IExecutable, Promise> _promiseByExecutable;

		protected AbstractExecutionSupervisor(ControllerBinder binder) :
				base(binder)
		{
		}

		protected bool TryGetPromise(IExecutable executable, out Promise promise)
		{
			return _promiseByExecutable.TryGetValue(executable, out promise);
		}

		protected Expected<IFuture> Execute<TAction>(ref TAction action, IActionDelegate<TAction> actionDelegate)
				where TAction : struct
		{
			try
			{
				if (!actionDelegate.Init(action))
				{
					IFuture future = FuturePool.PullRejected(new Error((int)ControllerErrorCode.InitializationFailed,
							$"Failed to initialize delegate '{actionDelegate.GetType().Name}' with action '{typeof(TAction).Name}'."));
					return new Expected<IFuture>(future);
				}
			}
			catch (Exception exception)
			{
				IFuture future = FuturePool.PullRejected(new Error((int)ControllerErrorCode.InitializationFailed,
						$"Failed to initialize delegate '{actionDelegate.GetType().Name}' with action '{typeof(TAction).Name}'.",
						exception));
				return new Expected<IFuture>(future);
			}
			return Execute(actionDelegate);
		}

		private Expected<IFuture> Execute(IExecutable executable)
		{
			if (!TryGetExecutionDelegate(executable, out IExecutionDelegate executionDelegate))
			{
				return new Expected<IFuture>(new Error(new ControllerException(ControllerErrorCode.ExecutorNotRegistered,
						$"Executor for '{executable.GetType().Name}' is not registered.")));
			}
			Promise promise = PromisePool.Pull();
			IFuture future = FuturePool.Pull(promise);
			if (_promiseByExecutable == null)
			{
				_promiseByExecutable = new Dictionary<IExecutable, Promise>{{ executable, promise }};
			}
			else
			{
				_promiseByExecutable.Add(executable, promise);
			}
			if (!TryExecute(executable, executionDelegate, out Error error))
			{
				_promiseByExecutable.Remove(executable);
				promise.Reject(error);
				HandleRelease(executable);
				PromisePool.Release(promise);
			}
			return new Expected<IFuture>(future);
		}

		protected override void HandleComplete(IExecutable executable)
		{
			if (!_promiseByExecutable.TryGetValue(executable, out Promise promise))
			{
				HandleRelease(executable);
				throw new ControllerException(ControllerErrorCode.UnexpectedExecutable,
						$"Unexpected executable '{executable.GetType().Name}'.");
			}
			_promiseByExecutable.Remove(executable);
			if (executable.IsSucceed)
			{
				promise.Resolve();
			}
			else
			{
				promise.Reject(executable.Error);
			}
			HandleRelease(executable);
			PromisePool.Release(promise);
		}

		protected abstract void HandleRelease(IExecutable executable);
	}

	internal abstract class AbstractExecutionSupervisor<TResult> : AbstractSupervisor
	{
		private Dictionary<IExecutable, Promise<TResult>> _promiseByExecutable;

		protected AbstractExecutionSupervisor(ControllerBinder binder) :
				base(binder)
		{
		}

		protected bool TryGetPromise(IExecutable executable, out Promise<TResult> promise)
		{
			return _promiseByExecutable.TryGetValue(executable, out promise);
		}

		protected Expected<IFuture<TResult>> Execute<TAction>(ref TAction action, IActionDelegate<TAction, TResult> actionDelegate)
				where TAction : struct
		{
			try
			{
				if (!actionDelegate.Init(action))
				{
					IFuture<TResult> future = FuturePool.PullRejected<TResult>(new Error((int)ControllerErrorCode.InitializationFailed,
							$"Failed to initialize delegate '{actionDelegate.GetType().Name}' with action '{typeof(TAction).Name}'."));
					return new Expected<IFuture<TResult>>(future);
				}
			}
			catch (Exception exception)
			{
				IFuture<TResult> future = FuturePool.PullRejected<TResult>(new Error((int)ControllerErrorCode.InitializationFailed,
						$"Failed to initialize delegate '{actionDelegate.GetType().Name}' with action '{typeof(TAction).Name}'.",
						exception));
				return new Expected<IFuture<TResult>>(future);
			}
			return Execute(actionDelegate);
		}

		private Expected<IFuture<TResult>> Execute(IExecutable<TResult> executable)
		{
			if (!TryGetExecutionDelegate(executable, out IExecutionDelegate executionDelegate))
			{
				return new Expected<IFuture<TResult>>(new Error(new ControllerException(ControllerErrorCode.ExecutorNotRegistered,
						$"Executor for '{executable.GetType().Name}' is not registered.")));
			}
			Promise<TResult> promise = PromisePool.Pull<TResult>();
			IFuture<TResult> future = FuturePool.Pull(promise);
			if (_promiseByExecutable == null)
			{
				_promiseByExecutable = new Dictionary<IExecutable, Promise<TResult>>{{ executable, promise }};
			}
			else
			{
				_promiseByExecutable.Add(executable, promise);
			}
			if (!TryExecute(executable, executionDelegate, out Error error))
			{
				_promiseByExecutable.Remove(executable);
				promise.Reject(error);
				HandleRelease(executable);
				PromisePool.Release(promise);
			}
			return new Expected<IFuture<TResult>>(future);
		}

		protected override void HandleComplete(IExecutable executable)
		{
			if (!_promiseByExecutable.TryGetValue(executable, out Promise<TResult> promise))
			{
				HandleRelease(executable);
				throw new ControllerException(ControllerErrorCode.UnexpectedExecutable,
						$"Unexpected executable '{executable.GetType().Name}'.");
			}
			_promiseByExecutable.Remove(executable);
			if (executable.IsSucceed)
			{
				if (executable is IExecutable<TResult> resultExecutable)
				{
					try
					{
						promise.Resolve(resultExecutable.Result);
					}
					catch (Exception exception)
					{
						promise.Reject(new Error((int)ControllerErrorCode.GetResultFailed,
								$"Failed to get result '{typeof(TResult).Name}' from '{executable.GetType().Name}'.",
								exception));
					}
				}
				else
				{
					promise.Reject(new Error((int)ControllerErrorCode.UnexpectedExecutable,
							$"Unexpected executable '{executable.GetType().Name}'. Has no result '{typeof(TResult).Name}'."));
				}
			}
			else
			{
				promise.Reject(executable.Error);
			}
			HandleRelease(executable);
			PromisePool.Release(promise);
		}

		protected abstract void HandleRelease(IExecutable executable);
	}
}