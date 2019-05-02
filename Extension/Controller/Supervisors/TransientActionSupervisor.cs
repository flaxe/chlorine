using System.Collections.Generic;
using Chlorine.Bindings;
using Chlorine.Exceptions;
using Chlorine.Execution;
using Chlorine.Pools;
using Chlorine.Providers;

namespace Chlorine.Supervisors
{
	internal class TransientActionSupervisor<TAction> :
			AbstractActionSupervisor<TAction>,
			IActionSupervisor<TAction>
			where TAction : struct
	{
		private readonly IActionDelegateProvider<IActionDelegate<TAction>> _provider;

		private Dictionary<IActionDelegate<TAction>, Promise> _promiseByDelegate;

		public TransientActionSupervisor(ControllerBinder binder, IActionDelegateProvider<IActionDelegate<TAction>> provider) :
				base(binder)
		{
			_provider = provider;
		}

		protected IReadOnlyDictionary<IActionDelegate<TAction>, Promise> Promises => _promiseByDelegate;

		public Expected<IPromise> Perform(ref TAction action)
		{
			if (TryPerform(ref action, out IPromise promise, out Error error))
			{
				return new Expected<IPromise>(promise);
			}
			return error;
		}

		protected virtual bool TryPerform(ref TAction action, out IPromise promise, out Error error)
		{
			Promise actionPromise = PromisePool.Pull();
			IActionDelegate<TAction> actionDelegate = _provider.Provide();
			if (_promiseByDelegate == null)
			{
				_promiseByDelegate = new Dictionary<IActionDelegate<TAction>, Promise>
				{
						{actionDelegate, actionPromise}
				};
			}
			else
			{
				_promiseByDelegate.Add(actionDelegate, actionPromise);
			}
			if (TryExecute(ref action, actionDelegate, out Error actionError))
			{
				promise = actionPromise;
				error = default;
				return true;
			}

			_promiseByDelegate.Remove(actionDelegate);
			_provider.Release(actionDelegate);
			PromisePool.Release(actionPromise);

			error = actionError;
			promise = default;
			return false;
		}

		public override void HandleComplete(IExecutable executable)
		{
			IActionDelegate<TAction> actionDelegate = (IActionDelegate<TAction>)executable;
			if (!_promiseByDelegate.TryGetValue(actionDelegate, out Promise promise))
			{
				_provider.Release(actionDelegate);
				throw new ControllerException(ControllerErrorCode.UnexpectedAction,
						$"Unexpected action delegate with action '{typeof(TAction).Name}'.");
			}
			try
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
			finally
			{
				_promiseByDelegate.Remove(actionDelegate);
				_provider.Release(actionDelegate);
				PromisePool.Release(promise);
			}
		}
	}

	internal class TransientActionSupervisor<TAction, TResult> :
			AbstractActionSupervisor<TAction>,
			IActionSupervisor<TAction, TResult>
			where TAction : struct
	{
		private readonly IActionDelegateProvider<IActionDelegate<TAction, TResult>> _provider;

		private Dictionary<IActionDelegate<TAction, TResult>, Promise<TResult>> _promiseByDelegate;

		public TransientActionSupervisor(ControllerBinder binder, IActionDelegateProvider<IActionDelegate<TAction, TResult>> provider) :
				base(binder)
		{
			_provider = provider;
		}

		protected IReadOnlyDictionary<IActionDelegate<TAction, TResult>, Promise<TResult>> Promises => _promiseByDelegate;

		public Expected<IPromise<TResult>> Perform(ref TAction action)
		{
			if (TryPerform(ref action, out IPromise<TResult> promise, out Error error))
			{
				return new Expected<IPromise<TResult>>(promise);
			}
			return error;
		}

		Expected<IPromise> IActionSupervisor<TAction>.Perform(ref TAction action)
		{
			if (TryPerform(ref action, out IPromise<TResult> promise, out Error error))
			{
				return new Expected<IPromise>(promise);
			}
			return error;
		}

		protected virtual bool TryPerform(ref TAction action, out IPromise<TResult> promise, out Error error)
		{
			Promise<TResult> actionPromise = PromisePool<TResult>.Pull();
			IActionDelegate<TAction, TResult> actionDelegate = _provider.Provide();
			if (_promiseByDelegate == null)
			{
				_promiseByDelegate = new Dictionary<IActionDelegate<TAction, TResult>, Promise<TResult>>
				{
						{actionDelegate, actionPromise}
				};
			}
			else
			{
				_promiseByDelegate.Add(actionDelegate, actionPromise);
			}
			if (TryExecute(ref action, actionDelegate, out Error actionError))
			{
				promise = actionPromise;
				error = default;
				return true;
			}

			_promiseByDelegate.Remove(actionDelegate);
			_provider.Release(actionDelegate);
			PromisePool<TResult>.Release(actionPromise);

			error = actionError;
			promise = default;
			return false;
		}

		public override void HandleComplete(IExecutable executable)
		{
			IActionDelegate<TAction, TResult> actionDelegate = (IActionDelegate<TAction, TResult>)executable;
			if (!_promiseByDelegate.TryGetValue(actionDelegate, out Promise<TResult> promise))
			{
				_provider.Release(actionDelegate);
				throw new ControllerException(ControllerErrorCode.UnexpectedAction,
						$"Unexpected action delegate with action '{typeof(TAction).Name}'.");
			}
			try
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
								"Failed to get result from action delegate."));
					}
				}
				else
				{
					promise.Reject(actionDelegate.Error);
				}
			}
			finally
			{
				_promiseByDelegate.Remove(actionDelegate);
				_provider.Release(actionDelegate);
				PromisePool<TResult>.Release(promise);
			}
		}
	}
}