using Chlorine.Controller.Bindings;
using Chlorine.Controller.Providers;
using Chlorine.Controller.Supervisors.Internal;
using Chlorine.Futures;

namespace Chlorine.Controller.Supervisors
{
	internal sealed class TransientActionSupervisor<TAction> :
			AbstractActionSupervisor<TAction>,
			IActionSupervisor<TAction>
			where TAction : struct
	{
		public TransientActionSupervisor(ControllerBinder binder, IProvider<IActionDelegate<TAction>> provider) :
				base(binder, provider)
		{
		}

		public Expected<IPromise> Perform(ref TAction action)
		{
			if (TryPerform(ref action, out Promise promise, out Error error))
			{
				return new Expected<IPromise>(promise);
			}
			return error;
		}
	}

	internal sealed class TransientActionSupervisor<TAction, TResult> :
			AbstractActionSupervisor<TAction, TResult>,
			IActionSupervisor<TAction, TResult>
			where TAction : struct
	{
		public TransientActionSupervisor(ControllerBinder binder, IProvider<IActionDelegate<TAction, TResult>> provider) :
				base(binder, provider)
		{
		}

		public Expected<IPromise<TResult>> Perform(ref TAction action)
		{
			if (TryPerform(ref action, out Promise<TResult> promise, out Error error))
			{
				return new Expected<IPromise<TResult>>(promise);
			}
			return error;
		}

		Expected<IPromise> IActionSupervisor<TAction>.Perform(ref TAction action)
		{
			if (TryPerform(ref action, out Promise<TResult> promise, out Error error))
			{
				return new Expected<IPromise>(promise);
			}
			return error;
		}
	}
}