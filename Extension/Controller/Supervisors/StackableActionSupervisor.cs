using Chlorine.Bindings;
using Chlorine.Providers;

namespace Chlorine.Supervisors
{
	internal class StackableActionSupervisor<TAction> :
			TransientActionSupervisor<TAction>
			where TAction : struct
	{
		public StackableActionSupervisor(ControllerBinder binder, IActionDelegateProvider<IActionDelegate<TAction>> provider) :
				base(binder, provider)
		{
		}

		protected override bool TryPerform(ref TAction action, out IPromise promise, out Error error)
		{
			foreach (var pair in Promises)
			{
				IActionDelegate<TAction> actionDelegate = pair.Key;
				if (actionDelegate.IsPending && actionDelegate is IStackable<TAction> stackable && stackable.Stack(action))
				{
					promise = pair.Value;
					error = default;
					return true;
				}
			}
			return base.TryPerform(ref action, out promise, out error);
		}
	}

	internal class StackableActionSupervisor<TAction, TResult> :
			TransientActionSupervisor<TAction, TResult>
			where TAction : struct
	{
		public StackableActionSupervisor(ControllerBinder binder, IActionDelegateProvider<IActionDelegate<TAction, TResult>> provider) :
				base(binder, provider)
		{
		}

		protected override bool TryPerform(ref TAction action, out IPromise<TResult> promise, out Error error)
		{
			foreach (var pair in Promises)
			{
				IActionDelegate<TAction, TResult> actionDelegate = pair.Key;
				if (actionDelegate.IsPending && actionDelegate is IStackable<TAction> stackable && stackable.Stack(action))
				{
					promise = pair.Value;
					error = default;
					return true;
				}
			}
			return base.TryPerform(ref action, out promise, out error);
		}
	}
}