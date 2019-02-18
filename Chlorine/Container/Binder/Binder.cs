using System;
using System.Collections.Generic;

namespace Chlorine
{
	internal class Binder
	{
		private readonly Dictionary<Type, IBindingProvider> _providerByType;
		private readonly Dictionary<Type, Dictionary<object, IBindingProvider>> _providerByTypeAndId;

		public Binder()
		{
			_providerByType = new Dictionary<Type, IBindingProvider>();
			_providerByTypeAndId = new Dictionary<Type, Dictionary<object, IBindingProvider>>();
		}

		public void Bind(Type type, object id, IBindingProvider provider)
		{
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

		public object Resolve(Type type, object id)
		{
			if (TryGetProvider(type, id, out IBindingProvider provider))
			{
				return provider.Provide();
			}
			return null;
		}

		private bool TryGetProvider(Type type, object id, out IBindingProvider provider)
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