using System;
using System.Collections.Generic;

namespace Chlorine.Controller
{
	internal class ControllerBinder
	{
		private readonly ControllerBinder _parent;

		private Dictionary<Type, IProvider> _actionDelegateProviderByType;
		private Dictionary<Type, IExecutionDelegate> _executionDelegateByType;

		public ControllerBinder(ControllerBinder parent = null)
		{
			_parent = parent;
		}

		public void BindAction<TAction>(IProvider<IActionDelegate<TAction>> provider)
				where TAction : struct
		{
			Type actionType = typeof(TAction);
			if (_actionDelegateProviderByType == null)
			{
				_actionDelegateProviderByType = new Dictionary<Type, IProvider> {{actionType, provider}};
			}
			else if (_actionDelegateProviderByType.ContainsKey(actionType))
			{
				throw new ArgumentException($"Action with type '{actionType.Name}' is already registered.");
			}
			else
			{
				_actionDelegateProviderByType.Add(actionType, provider);
			}
		}

		public void BindExecutable<TExecutable>(ExecutionDelegate<TExecutable> executionDelegate)
				where TExecutable : class, IExecutable
		{
			Type executableType = typeof(TExecutable);
			if (_executionDelegateByType == null)
			{
				_executionDelegateByType = new Dictionary<Type, IExecutionDelegate> {{executableType, executionDelegate}};
			}
			else if (_executionDelegateByType.ContainsKey(executableType))
			{
				throw new ArgumentException($"Executable with type '{executableType.Name}' is already registered.");
			}
			else
			{
				_executionDelegateByType.Add(executableType, executionDelegate);
			}
		}

		public bool TryResolveActionDelegate<TAction>(Type type, out IActionDelegate<TAction> actionDelegate)
				where TAction : struct
		{
			if (_actionDelegateProviderByType != null && _actionDelegateProviderByType.TryGetValue(type, out IProvider provider))
			{
				actionDelegate = provider.Provide() as IActionDelegate<TAction>;
				return true;
			}
			if (_parent != null && _parent.TryResolveActionDelegate(type, out actionDelegate))
			{
				return true;
			}
			actionDelegate = default;
			return false;
		}

		public bool TryResolveExecutionDelegate(IExecutable executable, out IExecutionDelegate executionDelegate)
		{
			Type executableType = executable.GetType();
			foreach (KeyValuePair<Type, IExecutionDelegate> pair in _executionDelegateByType)
			{
				if (executableType.IsEqualOrDerivesFrom(pair.Key))
				{
					executionDelegate = pair.Value;
					return true;
				}
			}
			executionDelegate = default;
			return false;
		}
	}
}