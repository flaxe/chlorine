using System;
using System.Collections.Generic;
using Chlorine.Exceptions;
using Chlorine.Providers;

namespace Chlorine.Bindings
{
	internal sealed class Binder : IDisposable
	{
		private readonly Container _container;
		private readonly Binder _parent;

		private readonly Dictionary<Type, IProvider> _providerByType;
		private readonly Dictionary<Type, Dictionary<object, IProvider>> _providerByTypeAndId;

		public Binder(Container container, Binder parent = null)
		{
			_container = container;
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
			return new BindingType<T>(_container, this);
		}

		public void Bind<T>(IProvider<T> provider) where T : class
		{
			Bind(null, provider);
		}

		public void Bind<T>(object id, IProvider<T> provider) where T : class
		{
			Type type = typeof(T);
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

		public bool TryResolveType<T>(object id, out T instance) where T : class
		{
			if (TryResolveType(typeof(T), id, out object value) && value is T concreteValue)
			{
				instance = concreteValue;
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