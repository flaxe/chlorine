using System;
using System.Collections.Generic;

namespace Chlorine
{
	internal class Binder
	{
		private readonly Binder _parentBinder;

		private Dictionary<Type, IProvider> _providerByType;
		private Dictionary<Type, Dictionary<object, IProvider>> _providerByTypeAndId;

		private Dictionary<Type, IProvider> _actionDelegateProviderByType;
		private Dictionary<Type, IExecutionDelegate> _executionDelegateByType;

		public Binder(Binder parentBinder)
		{
			_parentBinder = parentBinder;
		}

		public void Bind<T>(IProvider<T> provider)
				where T : class
		{
			Bind(null, provider);
		}

		public void Bind<T>(object id, IProvider<T> provider)
				where T : class
		{
			Type type = typeof(T);
			if (id == null)
			{
				if (_providerByType == null)
				{
					_providerByType = new Dictionary<Type, IProvider>{{type, provider}};
				}
				else if (_providerByType.ContainsKey(type))
				{
					throw new ArgumentException($"Type '{type.Name}' with empty id is already registered.");
				}
				else
				{
					_providerByType.Add(type, provider);
				}
			}
			else
			{
				if (_providerByTypeAndId == null)
				{
					_providerByTypeAndId = new Dictionary<Type, Dictionary<object, IProvider>>
					{
							{type, new Dictionary<object, IProvider>{{id, provider}}}
					};
				}
				else if (_providerByTypeAndId.TryGetValue(type, out Dictionary<object, IProvider> providerById))
				{
					if (providerById.ContainsKey(id))
					{
						throw new ArgumentException($"Type '{type.Name}' with id '{id}' is already registered.");
					}
					providerById.Add(id, provider);
				}
				else
				{
					_providerByTypeAndId.Add(type, new Dictionary<object, IProvider>{{id, provider}});
				}
			}
		}

		public void BindAction<TAction>(IProvider<IActionDelegate<TAction>> provider)
				where TAction : struct
		{
			Type actionType = typeof(TAction);
			if (_actionDelegateProviderByType == null)
			{
				_actionDelegateProviderByType = new Dictionary<Type, IProvider>{{actionType, provider}};
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
				_executionDelegateByType = new Dictionary<Type, IExecutionDelegate>{{executableType, executionDelegate}};
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

		public bool TryResolveType<T>(object id, out T instance) where T : class
		{
			if (TryResolveType(typeof(T), id, out object value))
			{
				instance = value as T;
				return true;
			}
			instance = default;
			return false;
		}

		public bool TryResolveType(Type type, object id, out object instance)
		{
			if (TryGetTypeProvider(type, id, out IProvider provider))
			{
				instance = provider.Provide();
				return true;
			}
			if (_parentBinder != null && _parentBinder.TryResolveType(type, id, out instance))
			{
				return true;
			}
			instance = default;
			return false;
		}

		public bool TryResolveActionDelegate<TAction>(Type type, out IActionDelegate<TAction> actionDelegate)
				where TAction : struct
		{
			if (_actionDelegateProviderByType != null && _actionDelegateProviderByType.TryGetValue(type, out IProvider provider))
			{
				actionDelegate = provider.Provide() as IActionDelegate<TAction>;
				return true;
			}
			if (_parentBinder != null && _parentBinder.TryResolveActionDelegate(type, out actionDelegate))
			{
				return true;
			}
			actionDelegate = default;
			return false;
		}

		public bool TryResolveExecutionDelegate(IExecutable executable, out IExecutionDelegate executionDelegate)
		{
			Type executableType = executable.GetType();
			foreach (KeyValuePair<Type,IExecutionDelegate> pair in _executionDelegateByType)
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

		private bool TryGetTypeProvider(Type type, object id, out IProvider provider)
		{
			if (id == null)
			{
				if (_providerByType != null && _providerByType.TryGetValue(type, out provider))
				{
					return true;
				}
			}
			else if (_providerByTypeAndId != null && _providerByTypeAndId.TryGetValue(type, out Dictionary<object, IProvider> providerById))
			{
				if (providerById.TryGetValue(id, out provider))
				{
					return true;
				}
			}
			provider = default;
			return false;
		}
	}
}