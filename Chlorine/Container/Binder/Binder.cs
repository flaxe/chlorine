using System;
using System.Collections.Generic;

namespace Chlorine
{
	internal class Binder
	{
		private readonly Dictionary<Type, IBindingProvider> _providerByType;
		private readonly Dictionary<Type, Dictionary<object, IBindingProvider>> _providerByTypeAndId;

		private readonly Dictionary<Type, IBindingProvider> _actionDelegateProviderByType;

		private readonly Binder _parentBinder;

		public Binder(Binder parentBinder)
		{
			_parentBinder = parentBinder;
			_providerByType = new Dictionary<Type, IBindingProvider>();
			_providerByTypeAndId = new Dictionary<Type, Dictionary<object, IBindingProvider>>();
			_actionDelegateProviderByType = new Dictionary<Type, IBindingProvider>();
		}

		public void Bind<T>(IBindingProvider provider) where T : class
		{
			Bind<T>(null, provider);
		}

		public void Bind<T>(object id, IBindingProvider provider) where T : class
		{
			Type type = typeof(T);
			if (id == null)
			{
				if (_providerByType.ContainsKey(type))
				{
					throw new ArgumentException($"Type '{type.Name}' with empty id already registered.");
				}
				_providerByType.Add(type, provider);
			}
			else
			{
				if (!_providerByTypeAndId.TryGetValue(type, out Dictionary<object, IBindingProvider> providerById))
				{
					providerById = new Dictionary<object, IBindingProvider>();
					_providerByTypeAndId.Add(type, providerById);
				}
				if (providerById.ContainsKey(id))
				{
					throw new ArgumentException($"Type '{type.Name}' with id '{id}' already registered.");
				}
				providerById.Add(id, provider);
			}
		}

		public void BindAction<TAction>(IBindingProvider provider) where TAction : struct
		{
			Type actionType = typeof(TAction);
			if (_actionDelegateProviderByType.ContainsKey(actionType))
			{
				throw new ArgumentException($"Action with type '{actionType.Name}' already registered.");
			}
			_actionDelegateProviderByType.Add(actionType, provider);
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
			if (TryGetTypeProvider(type, id, out IBindingProvider provider))
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

		public IActionDelegate<TAction> ResolveActionDelegate<TAction>() where TAction : struct
		{
			return ResolveActionDelegate(typeof(TAction)) as IActionDelegate<TAction>;
		}

		public object ResolveActionDelegate(Type actionType)
		{
			if (_actionDelegateProviderByType.TryGetValue(actionType, out IBindingProvider provider))
			{
				return provider.Provide();
			}
			return _parentBinder?.ResolveActionDelegate(actionType);
		}

		private bool TryGetTypeProvider(Type type, object id, out IBindingProvider provider)
		{
			if (id == null)
			{
				if (_providerByType.TryGetValue(type, out IBindingProvider bindingProvider))
				{
					provider = bindingProvider;
					return true;
				}
			}
			else if (_providerByTypeAndId.TryGetValue(type, out Dictionary<object, IBindingProvider> providerById))
			{
				if (providerById.TryGetValue(id, out IBindingProvider bindingProvider))
				{
					provider = bindingProvider;
					return true;
				}
			}
			provider = default;
			return false;
		}
	}
}