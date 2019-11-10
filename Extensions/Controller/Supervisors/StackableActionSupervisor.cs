using Chlorine.Controller.Bindings;
using Chlorine.Controller.Exceptions;
using Chlorine.Controller.Providers;
using Chlorine.Controller.Supervisors.Internal;
using Chlorine.Futures;

namespace Chlorine.Controller.Supervisors
{
	internal sealed class StackableActionSupervisor<TAction> :
			AbstractActionSupervisor<TAction>,
			IActionSupervisor<TAction>
			where TAction : struct
	{
		public StackableActionSupervisor(ControllerBinder binder, IProvider<IActionDelegate<TAction>> provider) :
				base(binder, provider)
		{
		}

		public Expected<IPromise> Perform(ref TAction action)
		{
			if (TryStack(ref action, out Promise stackPromise))
			{
				return new Expected<IPromise>(stackPromise);
			}
			if (TryPerform(ref action, out Promise performPromise, out Error error))
			{
				return new Expected<IPromise>(performPromise);
			}
			return error;
		}

		protected override Expected<IActionDelegate<TAction>> ProvideDelegate()
		{
			Expected<IActionDelegate<TAction>> expectedDelegate = base.ProvideDelegate();
			if (expectedDelegate.TryGetValue(out IActionDelegate<TAction> actionDelegate))
			{
				if (!(actionDelegate is IStackable<TAction>))
				{
					return new Error((int)ControllerErrorCode.InvalidDelegate,
							$"'{actionDelegate.GetType().Name}' is not stackable.");
				}
			}
			return expectedDelegate;
		}

		private bool TryStack(ref TAction action, out Promise promise)
		{
			if (CurrentDelegates != null)
			{
				foreach (IActionDelegate<TAction> currentDelegate in CurrentDelegates)
				{
					if (currentDelegate.IsPending &&
							currentDelegate is IStackable<TAction> stackable &&
							stackable.Stack(action) &&
							TryGetPromise(currentDelegate, out Promise currentPromise))
					{
						promise = currentPromise;
						return true;
					}
				}
			}
			promise = default;
			return false;
		}
	}

	internal sealed class StackableActionSupervisor<TAction, TResult> :
			AbstractActionSupervisor<TAction, TResult>,
			IActionSupervisor<TAction, TResult>
			where TAction : struct
	{
		public StackableActionSupervisor(ControllerBinder binder, IProvider<IActionDelegate<TAction, TResult>> provider) :
				base(binder, provider)
		{
		}

		public Expected<IPromise<TResult>> Perform(ref TAction action)
		{
			if (TryStack(ref action, out Promise<TResult> stackPromise))
			{
				return new Expected<IPromise<TResult>>(stackPromise);
			}
			if (TryPerform(ref action, out Promise<TResult> performPromise, out Error error))
			{
				return new Expected<IPromise<TResult>>(performPromise);
			}
			return error;
		}

		Expected<IPromise> IActionSupervisor<TAction>.Perform(ref TAction action)
		{
			if (TryStack(ref action, out Promise<TResult> stackPromise))
			{
				return new Expected<IPromise>(stackPromise);
			}
			if (TryPerform(ref action, out Promise<TResult> performPromise, out Error error))
			{
				return new Expected<IPromise>(performPromise);
			}
			return error;
		}

		protected override Expected<IActionDelegate<TAction, TResult>> ProvideDelegate()
		{
			Expected<IActionDelegate<TAction, TResult>> expectedDelegate = base.ProvideDelegate();
			if (expectedDelegate.TryGetValue(out IActionDelegate<TAction, TResult> actionDelegate))
			{
				if (!(actionDelegate is IStackable<TAction>))
				{
					return new Error((int)ControllerErrorCode.InvalidDelegate,
							$"'{actionDelegate.GetType().Name}' is not stackable.");
				}
			}
			return expectedDelegate;
		}

		private bool TryStack(ref TAction action, out Promise<TResult> promise)
		{
			if (CurrentDelegates != null)
			{
				foreach (IActionDelegate<TAction, TResult> currentDelegate in CurrentDelegates)
				{
					if (currentDelegate.IsPending &&
							currentDelegate is IStackable<TAction> stackable &&
							stackable.Stack(action) &&
							TryGetPromise(currentDelegate, out Promise<TResult> currentPromise))
					{
						promise = currentPromise;
						return true;
					}
				}
			}
			promise = default;
			return false;
		}
	}
}