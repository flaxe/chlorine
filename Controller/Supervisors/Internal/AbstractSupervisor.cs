using System;
using System.Diagnostics.CodeAnalysis;
using Carbone.Bindings;
using Carbone.Exceptions;
using Carbone.Execution;

namespace Carbone.Supervisors.Internal
{
	internal abstract class AbstractSupervisor : IExecutionHandler
	{
		private readonly ControllerBinder _binder;

		protected AbstractSupervisor(ControllerBinder binder)
		{
			_binder = binder;
		}

		protected bool TryGetExecutionDelegate(IExecutable executable, [NotNullWhen(true)] out IExecutionDelegate? executionDelegate)
		{
			Type executableType = executable.GetType();
			return _binder.TryResolveExecutionDelegate(executableType, out executionDelegate);
		}

		protected bool TryExecute(IExecutable executable, IExecutionDelegate executionDelegate, out Error error)
		{
			try
			{
				executionDelegate.Execute(executable, this);
			}
			catch (Exception exception)
			{
				error = new Error((int)ControllerErrorCode.ExecutionFailed,
						$"Failed to execute \"{executable.GetType().Name}\".", exception);
				return false;
			}
			error = default;
			return true;
		}

		protected abstract void HandleComplete(IExecutable executable);

		void IExecutionHandler.HandleExecutable(IExecutable executable)
		{
			HandleComplete(executable);
		}
	}
}