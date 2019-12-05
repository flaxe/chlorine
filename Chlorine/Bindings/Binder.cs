using System;
using System.Collections.Generic;
using Chlorine.Exceptions;
using Chlorine.Providers;

namespace Chlorine.Bindings
{
	internal sealed class Binder : IDisposable
	{
		private readonly WeakReference<Container> _container;
		private readonly Binder _parent;

		private readonly Dictionary<Type, IProvider> _providerByType;
		private readonly Dictionary<Type, Dictionary<object, IProvider>> _providerByTypeAndId;

#if DEBUG
		private Type _bindingType;
#endif

		public Binder(Container container, Binder parent = null)
		{
			_container = new WeakReference<Container>(container);
			_parent = parent;
			_providerByType = new Dictionary<Type, IProvider>();
			_providerByTypeAndId = new Dictionary<Type, Dictionary<object, IProvider>>();
		}

		~Binder()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_providerByType.Count > 0)
			{
				foreach (IProvider provider in _providerByType.Values)
				{
					if (provider is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
				_providerByType.Clear();
			}
			if (_providerByTypeAndId.Count > 0)
			{
				foreach (Dictionary<object, IProvider> providerById in _providerByTypeAndId.Values)
				{
					foreach (IProvider provider in providerById.Values)
					{
						if (provider is IDisposable disposable)
						{
							disposable.Dispose();
						}
					}
				}
				_providerByTypeAndId.Clear();
			}
		}

		public BindingType<T> Bind<T>() where T : class
		{
			if (!_container.TryGetTarget(out Container container))
			{
				throw new ContainerException(ContainerErrorCode.InvalidOperation,
						"Invalid operation. Container was finalized.");
			}
#if DEBUG
			if (_bindingType != null)
			{
				throw new ContainerException(ContainerErrorCode.IncompleteBinding,
						$"Incomplete binding. Finish '{_bindingType.Name}' binding.");
			}
			_bindingType = typeof(T);
#endif
			return new BindingType<T>(container, this);
		}

		internal void Register(Type type, IProvider provider)
		{
			Register(type, null, provider);
		}

		internal void Register(Type type, object id, IProvider provider)
		{
#if DEBUG
			if (_bindingType != null && _bindingType != type)
			{
				throw new ContainerException(ContainerErrorCode.UnexpectedBinding,
						$"Unexpected '{type.Name}'. Complete '{_bindingType.Name}' binding first.");
			}
			_bindingType = null;
#endif
			if (id == null)
			{
				if (_providerByType.ContainsKey(type))
				{
					throw new ContainerException(ContainerErrorCode.TypeAlreadyRegistered,
							$"Type '{type.Name}' with empty id is already registered.");
				}
				_providerByType.Add(type, provider);
			}
			else
			{
				if (_providerByTypeAndId.TryGetValue(type, out Dictionary<object, IProvider> providerById))
				{
					if (providerById.ContainsKey(id))
					{
						throw new ContainerException(ContainerErrorCode.TypeAlreadyRegistered,
								$"Type '{type.Name}' with id '{id}' is already registered.");
					}
					providerById.Add(id, provider);
				}
				else
				{
					_providerByTypeAndId.Add(type, new Dictionary<object, IProvider> {{id, provider}});
				}
			}
		}

		internal bool TryResolveType(Type type, object id, out object instance)
		{
#if DEBUG
			if (_bindingType != null)
			{
				throw new ContainerException(ContainerErrorCode.IncompleteBinding,
						$"Incomplete binding. Complete '{_bindingType.Name}' binding first");
			}
#endif
			if (TryGetTypeProvider(type, id, out IProvider provider))
			{
				instance = provider.Provide();
				return true;
			}
			if (_parent != null && _parent.TryResolveType(type, id, out instance))
			{
				return true;
			}
			instance = default;
			return false;
		}

		private bool TryGetTypeProvider(Type type, object id, out IProvider provider)
		{
			if (id == null)
			{
				if (_providerByType.TryGetValue(type, out provider))
				{
					return true;
				}
			}
			else if (_providerByTypeAndId.TryGetValue(type, out Dictionary<object, IProvider> providerById))
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