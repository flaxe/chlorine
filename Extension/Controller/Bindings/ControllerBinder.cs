using System;
using System.Collections.Generic;
using Chlorine.Action;
using Chlorine.Execution;

namespace Chlorine.Bindings
{
	internal sealed class ControllerBinder : IDisposable
	{
		private readonly ControllerBinder _parent;

		private Dictionary<Type, object> _actionSupervisorByType;
		private Dictionary<Type, IExecutionDelegate> _executionDelegateByType;

		private Dictionary<Type, IExecutionDelegate> _executorsCache;
		private HashSet<Type> _missingExecutorsCache;

		public ControllerBinder(ControllerBinder parent = null)
		{
			_parent = parent;
		}

		~ControllerBinder()
		{
			Dispose();
		}

		public void Dispose()
		{
		}

		public void BindAction<TAction>(IActionSupervisor<TAction> actionSupervisor)
				where TAction : struct
		{
			Type actionType = typeof(TAction);
			if (_actionSupervisorByType == null)
			{
				_actionSupervisorByType = new Dictionary<Type, object> {{actionType, actionSupervisor}};
			}
			else if (_actionSupervisorByType.ContainsKey(actionType))
			{
				throw new ArgumentException($"Action with type '{actionType.Name}' is already registered.");
			}
			else
			{
				_actionSupervisorByType.Add(actionType, actionSupervisor);
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

		public bool TryResolveActionSupervisor<TAction>(out IActionSupervisor<TAction> actionSupervisor)
				where TAction : struct
		{
			if (TryResolveActionSupervisor(typeof(TAction), out object value) && value is IActionSupervisor<TAction> concreteSupervisor)
			{
				actionSupervisor = concreteSupervisor;
				return true;
			}
			actionSupervisor = default;
			return false;
		}

		public bool TryResolveActionSupervisor(Type actionType, out object actionSupervisor)
		{
			if (_actionSupervisorByType != null && _actionSupervisorByType.TryGetValue(actionType, out actionSupervisor))
			{
				return true;
			}
			if (_parent != null && _parent.TryResolveActionSupervisor(actionType, out actionSupervisor))
			{
				return true;
			}
			actionSupervisor = default;
			return false;
		}

		public bool TryResolveExecutionDelegate(IExecutable executable, out IExecutionDelegate executionDelegate)
		{
			return TryResolveExecutionDelegate(executable.GetType(), out executionDelegate);
		}

		public bool TryResolveExecutionDelegate(Type executableType, out IExecutionDelegate executionDelegate)
		{
			if (_executorsCache != null && _executorsCache.TryGetValue(executableType, out executionDelegate))
			{
				return true;
			}
			if (_executionDelegateByType != null && (_missingExecutorsCache == null || !_missingExecutorsCache.Contains(executableType)))
			{
				foreach (KeyValuePair<Type, IExecutionDelegate> pair in _executionDelegateByType)
				{
					if (executableType.IsEqualOrDerivesFrom(pair.Key))
					{
						executionDelegate = pair.Value;
						if (_executorsCache == null)
						{
							_executorsCache = new Dictionary<Type, IExecutionDelegate> {{executableType, executionDelegate}};
						}
						else
						{
							_executorsCache.Add(executableType, executionDelegate);
						}
						return true;
					}
				}
				if (_missingExecutorsCache == null)
				{
					_missingExecutorsCache = new HashSet<Type> {executableType};
				}
				else
				{
					_missingExecutorsCache.Add(executableType);
				}
			}
			if (_parent != null && _parent.TryResolveExecutionDelegate(executableType, out executionDelegate))
			{
				return true;
			}
			executionDelegate = default;
			return false;
		}
	}
}