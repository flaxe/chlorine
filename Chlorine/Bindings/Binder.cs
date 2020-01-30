using System;
using System.Collections.Generic;
using Chlorine.Exceptions;
using Chlorine.Injection;
using Chlorine.Providers;

namespace Chlorine.Bindings
{
	internal sealed class Binder : IDisposable
	{
		private readonly WeakReference<Container> _container;
		private readonly Binder _parent;

		private readonly Dictionary<Type, IBindingProvider> _providerByType;
		private readonly Dictionary<Type, Dictionary<object, IBindingProvider>> _providerByTypeAndId;

#if DEBUG
		private Type _bindingType;
#endif

		public Binder(Container container, Binder parent = null)
		{
			_container = new WeakReference<Container>(container);
			_parent = parent;
			_providerByType = new Dictionary<Type, IBindingProvider>();
			_providerByTypeAndId = new Dictionary<Type, Dictionary<object, IBindingProvider>>();
		}

		~Binder()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_providerByType.Count > 0)
			{
				foreach (IBindingProvider provider in _providerByType.Values)
				{
					provider.Dispose();
				}
				_providerByType.Clear();
			}
			if (_providerByTypeAndId.Count > 0)
			{
				foreach (Dictionary<object, IBindingProvider> providerById in _providerByTypeAndId.Values)
				{
					foreach (IBindingProvider provider in providerById.Values)
					{
						provider.Dispose();
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

		internal void Register(Type type, object id, BindingCondition condition, IProvider provider)
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
				if (condition == null)
				{
					if (_providerByType.ContainsKey(type))
					{
						throw new ContainerException(ContainerErrorCode.TypeAlreadyRegistered,
								$"Type '{type.Name}' with empty id is already registered.");
					}
					_providerByType.Add(type, new PermanentProvider(provider));
				}
				else if (_providerByType.TryGetValue(type, out IBindingProvider bindingProvider))
				{
					if (!(bindingProvider is ConditionalProvider conditionalProvider) ||
							!conditionalProvider.TryRegister(condition, provider))
					{
						throw new ContainerException(ContainerErrorCode.TypeAlreadyRegistered,
								$"Type '{type.Name}' with empty id is already registered.");
					}
				}
				else
				{
					_providerByType.Add(type, new ConditionalProvider(condition, provider));
				}
			}
			else
			{
				if (_providerByTypeAndId.TryGetValue(type, out Dictionary<object, IBindingProvider> providerById))
				{
					if (condition == null)
					{
						if (providerById.ContainsKey(id))
						{
							throw new ContainerException(ContainerErrorCode.TypeAlreadyRegistered,
									$"Type '{type.Name}' with id '{id}' is already registered.");
						}
						providerById.Add(id, new PermanentProvider(provider));
					}
					else if (providerById.TryGetValue(id, out IBindingProvider bindingProvider))
					{
						if (!(bindingProvider is ConditionalProvider conditionalProvider) ||
								!conditionalProvider.TryRegister(condition, provider))
						{
							throw new ContainerException(ContainerErrorCode.TypeAlreadyRegistered,
									$"Type '{type.Name}' with id '{id}' is already registered.");
						}
					}
					else
					{
						providerById.Add(type, new ConditionalProvider(condition, provider));
					}
				}
				else
				{
					if (condition == null)
					{
						_providerByTypeAndId.Add(type, new Dictionary<object, IBindingProvider>
						{
								{id, new PermanentProvider(provider)}
						});
					}
					else
					{
						_providerByTypeAndId.Add(type, new Dictionary<object, IBindingProvider>
						{
								{id, new ConditionalProvider(condition, provider)}
						});
					}
				}
			}
		}

		internal bool TryResolveType(in InjectContext context, out object instance)
		{
#if DEBUG
			if (_bindingType != null)
			{
				throw new ContainerException(ContainerErrorCode.IncompleteBinding,
						$"Incomplete binding. Complete '{_bindingType.Name}' binding first");
			}
#endif
			Type type = context.InjectType;
			object id = context.InjectId;
			if (TryGetTypeProvider(type, id, out IBindingProvider provider) &&
					provider.TryProvide(in context, out instance))
			{
				return true;
			}
			if (_parent != null && _parent.TryResolveType(in context, out instance))
			{
				return true;
			}
			instance = default;
			return false;
		}

		private bool TryGetTypeProvider(Type type, object id, out IBindingProvider provider)
		{
			if (id == null)
			{
				if (_providerByType.TryGetValue(type, out provider))
				{
					return true;
				}
			}
			else if (_providerByTypeAndId.TryGetValue(type, out Dictionary<object, IBindingProvider> providerById))
			{
				if (providerById.TryGetValue(id, out provider))
				{
					return true;
				}
			}
			provider = default;
			return false;
		}

		private interface IBindingProvider : IDisposable
		{
			bool TryProvide(in InjectContext context, out object instance);
		}

		private sealed class PermanentProvider : IBindingProvider
		{
			private readonly IProvider _provider;

			public PermanentProvider(IProvider provider)
			{
				_provider = provider;
			}

			public void Dispose()
			{
				if (_provider is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}

			public bool TryProvide(in InjectContext context, out object instance)
			{
				instance = _provider.Provide();
				return true;
			}
		}

		private sealed class ConditionalProvider : IBindingProvider
		{
			private readonly Dictionary<BindingCondition, IProvider> _providerByCondition;

			public ConditionalProvider(BindingCondition condition, IProvider provider)
			{
				_providerByCondition = new Dictionary<BindingCondition, IProvider>
				{
						{condition, provider}
				};
			}

			public void Dispose()
			{
				foreach (KeyValuePair<BindingCondition, IProvider> providerPair in _providerByCondition)
				{
					if (providerPair.Value is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
			}

			public bool TryRegister(BindingCondition condition, IProvider provider)
			{
				if (_providerByCondition.ContainsKey(condition))
				{
					return false;
				}
				_providerByCondition.Add(condition, provider);
				return true;
			}

			public bool TryProvide(in InjectContext context, out object instance)
			{
				foreach (KeyValuePair<BindingCondition,IProvider> providerPair in _providerByCondition)
				{
					if (providerPair.Key.Invoke(in context))
					{
						instance = providerPair.Value.Provide();
						return true;
					}
				}
				instance = default;
				return false;
			}
		}
	}
}