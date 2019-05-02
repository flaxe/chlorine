using System;
using Chlorine.Bindings;
using Chlorine.Exceptions;
using Chlorine.Execution;

namespace Chlorine.Supervisors
{
	internal abstract class AbstractActionSupervisor<TAction> : IExecutionHandler
			where TAction : struct
	{
		protected static readonly Type ActionType = typeof(TAction);

		private readonly ControllerBinder _binder;

		protected AbstractActionSupervisor(ControllerBinder binder)
		{
			_binder = binder;
		}

		protected bool TryExecute(ref TAction action, IActionDelegate<TAction> actionDelegate, out Error error)
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

		public abstract void HandleComplete(IExecutable executable);
	}
}