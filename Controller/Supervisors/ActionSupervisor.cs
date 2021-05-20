using System;
using Carbone.Bindings;
using Carbone.Exceptions;
using Carbone.Execution;
using Carbone.Futures;
using Carbone.Providers;
using Carbone.Supervisors.Internal;

namespace Carbone.Supervisors
{
	internal sealed class ActionSupervisor<TAction> :
			AbstractExecutionSupervisor,
			IActionSupervisor<TAction>,
			IDisposable
			where TAction : struct
	{
		private readonly IDelegateProvider _delegateProvider;

		public ActionSupervisor(ControllerBinder binder, IDelegateProvider delegateProvider) :
				base(binder)
		{
			_delegateProvider = delegateProvider;
		}

		~ActionSupervisor()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_delegateProvider is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		public Expected<IFuture> Perform(in TAction action)
		{
			if (_delegateProvider.Provide() is IActionDelegate<TAction> actionDelegate)
			{
				return Execute(in action, actionDelegate);
			}
			throw new ControllerException(ControllerErrorCode.InvalidType,
					$"Delegate has invalid type, \"{typeof(IActionDelegate<TAction>).Name}\" is expected.");
		}

		protected override void HandleRelease(IExecutable executable)
		{
			_delegateProvider.Release(executable);
		}
	}

	internal sealed class ActionSupervisor<TAction, TResult> :
			AbstractExecutionSupervisor<TResult>,
			IActionSupervisor<TAction, TResult>,
			IDisposable
			where TAction : struct
	{
		private readonly IDelegateProvider _delegateProvider;

		public ActionSupervisor(ControllerBinder binder, IDelegateProvider delegateProvider) :
				base(binder)
		{
			_delegateProvider = delegateProvider;
		}

		~ActionSupervisor()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_delegateProvider is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		public Expected<IFuture<TResult>> Perform(in TAction action)
		{
			if (_delegateProvider.Provide() is IActionDelegate<TAction, TResult> actionDelegate)
			{
				return Execute(in action, actionDelegate);
			}
			throw new ControllerException(ControllerErrorCode.InvalidType,
					$"Delegate has invalid type, \"{typeof(IActionDelegate<TAction, TResult>).Name}\" is expected.");
		}

		Expected<IFuture> IActionSupervisor<TAction>.Perform(in TAction action)
		{
			Expected<IFuture<TResult>> expected = Perform(in action);
			return expected.TryGetValue(out IFuture<TResult> future) ?
					new Expected<IFuture>(future) :
					new Expected<IFuture>(expected.Error);
		}

		protected override void HandleRelease(IExecutable executable)
		{
			_delegateProvider.Release(executable);
		}
	}
}