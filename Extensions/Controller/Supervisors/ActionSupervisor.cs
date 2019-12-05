using System;
using Chlorine.Controller.Bindings;
using Chlorine.Controller.Execution;
using Chlorine.Controller.Providers;
using Chlorine.Controller.Supervisors.Internal;
using Chlorine.Futures;

namespace Chlorine.Controller.Supervisors
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

		public Expected<IFuture> Perform(ref TAction action)
		{
			return Execute(ref action, _delegateProvider.Provide() as IActionDelegate<TAction>);
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

		public Expected<IFuture<TResult>> Perform(ref TAction action)
		{
			return Execute(ref action, _delegateProvider.Provide() as IActionDelegate<TAction, TResult>);
		}

		Expected<IFuture> IActionSupervisor<TAction>.Perform(ref TAction action)
		{
			Expected<IFuture<TResult>> expected = Perform(ref action);
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