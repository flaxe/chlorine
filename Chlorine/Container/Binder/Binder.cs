using System;
using System.Collections.Generic;

namespace Chlorine
{
	internal class Binder
	{
		private readonly Binder _parent;

		private Dictionary<Type, IProvider> _providerByType;
		private Dictionary<Type, Dictionary<object, IProvider>> _providerByTypeAndId;

		public Binder(Binder parent = null)
		{
			_parent = parent;
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