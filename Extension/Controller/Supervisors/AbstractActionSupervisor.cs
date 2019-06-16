using Chlorine.Bindings;
using Chlorine.Exceptions;
using Chlorine.Pools;
using Chlorine.Providers;

namespace Chlorine.Supervisors
{
	internal abstract class AbstractActionSupervisor<TAction> :
			AbstractActionDelegateSupervisor<TAction, IActionDelegate<TAction>, Promise>
			where TAction : struct
	{
		protected AbstractActionSupervisor(ControllerBinder binder, IProvider<IActionDelegate<TAction>> provider) :
				base(binder, provider)
		{
		}

		protected sealed override void HandleComplete(IActionDelegate<TAction> actionDelegate, Promise promise)
		{
			if (actionDelegate.IsSucceed)
			{
				promise.Resolve();
			}
			else
			{
				promise.Reject(actionDelegate.Error);
			}
		}

		protected sealed override Promise Pull()
		{
			return PromisePool.Pull();
		}

		protected sealed override void Release(Promise promise)
		{
			PromisePool.Release(promise);
		}
	}

	internal abstract class AbstractActionSupervisor<TAction, TResult> :
			AbstractActionDelegateSupervisor<TAction, IActionDelegate<TAction, TResult>, Promise<TResult>>
			where TAction : struct
	{
		protected AbstractActionSupervisor(ControllerBinder binder, IProvider<IActionDelegate<TAction, TResult>> provider) :
				base(binder, provider)
		{
		}

		protected sealed override void HandleComplete(IActionDelegate<TAction, TResult> actionDelegate, Promise<TResult> promise)
		{
			if (actionDelegate.IsSucceed)
			{
				if (actionDelegate.TryGetResult(out TResult result))
				{
					promise.Resolve(result);
				}
				else
				{
					promise.Reject(new Error((int)ControllerErrorCode.GetResultFailed,
							$"Failed to get result '{typeof(TResult).Name}' from '{actionDelegate.GetType().Name}'."));
				}
			}
			else
			{
				promise.Reject(actionDelegate.Error);
			}
		}

		protected sealed override Promise<TResult> Pull()
		{
			return PromisePool<TResult>.Pull();
		}

		protected sealed override void Release(Promise<TResult> promise)
		{
			PromisePool<TResult>.Release(promise);
		}
	}
}