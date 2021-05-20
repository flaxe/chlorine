using System;
using System.Collections.Generic;
using Carbone.Bindings;
using Carbone.Exceptions;
using Carbone.Execution;
using Carbone.Futures;
using Carbone.Providers;
using Carbone.Supervisors.Internal;

namespace Carbone.Supervisors
{
	internal sealed class StackableActionSupervisor<TAction> :
			AbstractExecutionSupervisor,
			IActionSupervisor<TAction>,
			IDisposable
			where TAction : struct
	{
		private readonly IDelegateProvider _delegateProvider;
		private List<IStackableActionDelegate<TAction>>? _delegates;

		public StackableActionSupervisor(ControllerBinder binder, IDelegateProvider delegateProvider) :
				base(binder)
		{
			_delegateProvider = delegateProvider;
		}

		~StackableActionSupervisor()
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
			if (_delegates != null && _delegates.Count > 0)
			{
				foreach (IStackableActionDelegate<TAction> pendingDelegate in _delegates)
				{
					if (pendingDelegate.IsPending && pendingDelegate.Stack(action) &&
							TryGetPromise(pendingDelegate, out Promise? promise))
					{
						return new Expected<IFuture>(FuturePool.Pull(promise));
					}
				}
			}
			if (_delegateProvider.Provide() is IStackableActionDelegate<TAction> actionDelegate)
			{
				if (_delegates == null)
				{
					_delegates = new List<IStackableActionDelegate<TAction>>{ actionDelegate };
				}
				else
				{
					_delegates.Add(actionDelegate);
				}
				return Execute(in action, actionDelegate);
			}
			throw new ControllerException(ControllerErrorCode.InvalidType,
					$"Delegate has invalid type, \"{typeof(IStackableActionDelegate<TAction>).Name}\" is expected.");
		}

		protected override void HandleRelease(IExecutable executable)
		{
			if (_delegates == null || !(executable is IStackableActionDelegate<TAction> actionDelegate))
			{
				_delegateProvider.Release(executable);
				throw new ControllerException(ControllerErrorCode.UnexpectedExecutable,
						$"Unexpected executable \"{executable.GetType().Name}\".");
			}
			_delegates.Remove(actionDelegate);
			_delegateProvider.Release(executable);
		}
	}

	internal sealed class StackableActionSupervisor<TAction, TResult> :
			AbstractExecutionSupervisor<TResult>,
			IActionSupervisor<TAction, TResult>,
			IDisposable
			where TAction : struct
	{
		private readonly IDelegateProvider _delegateProvider;
		private List<IStackableActionDelegate<TAction, TResult>>? _delegates;

		public StackableActionSupervisor(ControllerBinder binder, IDelegateProvider delegateProvider) :
				base(binder)
		{
			_delegateProvider = delegateProvider;
		}

		~StackableActionSupervisor()
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
			if (_delegates != null && _delegates.Count > 0)
			{
				foreach (IStackableActionDelegate<TAction, TResult> pendingDelegate in _delegates)
				{
					if (pendingDelegate.IsPending && pendingDelegate.Stack(action) &&
							TryGetPromise(pendingDelegate, out Promise<TResult>? promise))
					{
						return new Expected<IFuture<TResult>>(FuturePool.Pull(promise));
					}
				}
			}
			if (_delegateProvider.Provide() is IStackableActionDelegate<TAction, TResult> actionDelegate)
			{
				if (_delegates == null)
				{
					_delegates = new List<IStackableActionDelegate<TAction, TResult>>{ actionDelegate };
				}
				else
				{
					_delegates.Add(actionDelegate);
				}
				return Execute(in action, actionDelegate);
			}
			throw new ControllerException(ControllerErrorCode.InvalidType,
					$"Delegate has invalid type, \"{typeof(IStackableActionDelegate<TAction, TResult>).Name}\" is expected.");
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
			if (_delegates == null || !(executable is IStackableActionDelegate<TAction, TResult> actionDelegate))
			{
				_delegateProvider.Release(executable);
				throw new ControllerException(ControllerErrorCode.UnexpectedExecutable,
						$"Unexpected executable \"{executable.GetType().Name}\".");
			}
			_delegates.Remove(actionDelegate);
			_delegateProvider.Release(executable);
		}
	}
}