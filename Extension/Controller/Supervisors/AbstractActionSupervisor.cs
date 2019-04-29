using System;
using Chlorine.Bindings;
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

		protected bool TryPerform(ref TAction action, IActionDelegate<TAction> actionDelegate, out Error error)
		{
			try
			{
				if (!actionDelegate.Init(action))
				{
					Type delegateType = actionDelegate.GetType();
					error = new Error($"Failed to initialize delegate '{delegateType.Name}' with action '{ActionType.Name}'.");
					return false;
				}
			}
			catch (Exception exception)
			{
				error = new Error(exception);
				return false;
			}
			if (!_binder.TryResolveExecutionDelegate(actionDelegate, out IExecutionDelegate executionDelegate))
			{
				Type delegateType = actionDelegate.GetType();
				error = new Error($"Failed to resolve executor for action delegate '{delegateType.Name}'.");
				return false;
			}
			try
			{
				executionDelegate.Execute(actionDelegate, this);
			}
			catch (Exception exception)
			{
				error = new Error(exception);
				return false;
			}
			error = default;
			return true;
		}

		public abstract void HandleComplete(IExecutable executable);
	}
}