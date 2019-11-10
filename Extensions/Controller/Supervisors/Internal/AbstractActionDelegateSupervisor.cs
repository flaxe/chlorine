using System;
using System.Collections.Generic;
using Chlorine.Controller.Bindings;
using Chlorine.Controller.Exceptions;
using Chlorine.Controller.Execution;
using Chlorine.Controller.Providers;
using Chlorine.Futures;

namespace Chlorine.Controller.Supervisors.Internal
{
	internal abstract class AbstractActionDelegateSupervisor<TAction, TActionDelegate, TPromise> : IExecutionHandler
			where TAction : struct
			where TActionDelegate : class, IActionDelegate<TAction>
			where TPromise : class, IPromise
	{
		private static readonly Type ActionType = typeof(TAction);

		private readonly ControllerBinder _binder;
		private readonly IProvider<TActionDelegate> _provider;

		private Dictionary<TActionDelegate, TPromise> _promiseByActionDelegate;

		protected AbstractActionDelegateSupervisor(ControllerBinder binder, IProvider<TActionDelegate> provider)
		{
			_binder = binder;
			_provider = provider;
		}

		protected IEnumerable<TActionDelegate> CurrentDelegates => _promiseByActionDelegate?.Keys;

		protected bool TryGetPromise(TActionDelegate actionDelegate, out TPromise promise)
		{
			return _promiseByActionDelegate.TryGetValue(actionDelegate, out promise);
		}

		protected bool TryPerform(ref TAction action, out TPromise promise, out Error error)
		{
			TPromise actionPromise = Pull();
			Expected<TActionDelegate> expectedDelegate = ProvideDelegate();
			if (expectedDelegate.TryGetValue(out TActionDelegate actionDelegate))
			{
				if (_promiseByActionDelegate == null)
				{
					_promiseByActionDelegate = new Dictionary<TActionDelegate, TPromise>
					{
							{actionDelegate, actionPromise}
					};
				}
				else
				{
					_promiseByActionDelegate.Add(actionDelegate, actionPromise);
				}
				if (TryExecute(ref action, actionDelegate, out error))
				{
					promise = actionPromise;
					error = default;
					return true;
				}

				_promiseByActionDelegate.Remove(actionDelegate);
			}
			else
			{
				error = expectedDelegate.Error;
			}

			Release(actionDelegate);
			Release(actionPromise);

			promise = default;
			return false;
		}

		protected virtual Expected<TActionDelegate> ProvideDelegate()
		{
			return new Expected<TActionDelegate>(_provider.Provide());
		}

		protected abstract void HandleComplete(TActionDelegate actionDelegate, TPromise promise);

		protected abstract TPromise Pull();
		protected abstract void Release(TPromise promise);

		private void Release(TActionDelegate actionDelegate)
		{
			if (_provider is IFromPoolProvider<TActionDelegate> poolProvider)
			{
				poolProvider.Release(actionDelegate);
			}
		}

		private bool TryExecute(ref TAction action, TActionDelegate actionDelegate, out Error error)
		{
			try
			{
				if (!actionDelegate.Init(action))
				{
					Type delegateType = actionDelegate.GetType();
					error = new Error((int)ControllerErrorCode.InitializationFailed,
							$"Failed to initialize delegate '{delegateType.Name}' with action '{ActionType.Name}'.");
					return false;
				}
			}
			catch (Exception exception)
			{
				Type delegateType = actionDelegate.GetType();
				error = new Error((int)ControllerErrorCode.InitializationFailed,
						$"Failed to initialize delegate '{delegateType.Name}' with action '{ActionType.Name}'.",
						exception);
				return false;
			}
			if (!_binder.TryResolveExecutionDelegate(actionDelegate, out IExecutionDelegate executionDelegate))
			{
				Type delegateType = actionDelegate.GetType();
				error = new Error((int)ControllerErrorCode.ExecutorNotRegistered,
						$"Executor for action delegate '{delegateType.Name}' not registered.");
				return false;
			}
			try
			{
				executionDelegate.Execute(actionDelegate, this);
			}
			catch (Exception exception)
			{
				Type delegateType = actionDelegate.GetType();
				error = new Error((int)ControllerErrorCode.ExecutionFailed,
						$"Failed to execute action delegate '{delegateType.Name}'.",
						exception);
				return false;
			}
			error = default;
			return true;
		}

		void IExecutionHandler.HandleComplete(IExecutable executable)
		{
			TActionDelegate actionDelegate = (TActionDelegate)executable;
			if (!_promiseByActionDelegate.TryGetValue(actionDelegate, out TPromise actionPromise))
			{
				Release(actionDelegate);
				throw new ControllerException(ControllerErrorCode.UnexpectedAction,
						$"Unexpected action delegate with action '{typeof(TAction).Name}'.");
			}
			try
			{
				HandleComplete(actionDelegate, actionPromise);
			}
			finally
			{
				_promiseByActionDelegate.Remove(actionDelegate);
				Release(actionDelegate);
				Release(actionPromise);
			}
		}
	}
}