using System;
using System.Collections.Generic;

namespace Chlorine.Extension
{
	internal sealed class Extender : IDisposable
	{
		private readonly Container _container;
		private readonly Extender _parent;

		private Dictionary<Type, IExtensionHolder> _holderByType;

		public Extender(Container container, Extender parent = null)
		{
			_container = container;
			_parent = parent;
		}

		~Extender()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_holderByType != null && _holderByType.Count > 0)
			{
				foreach (IExtensionHolder extensionHolder in _holderByType.Values)
				{
					extensionHolder.Dispose();
				}
			}
		}

		public bool TryGetExtension<TExtension>(out TExtension extension)
				where TExtension : class, IExtension<TExtension>, new()
		{
			Type extensionType = typeof(TExtension);
			if (_holderByType != null && _holderByType.TryGetValue(extensionType, out IExtensionHolder extensionHolder))
			{
				if (extensionHolder.Extension is TExtension targetExtension)
				{
					extension = targetExtension;
					return true;
				}
			}
			extension = default;
			return false;
		}

		public void Extend(Container container)
		{
			if (_holderByType != null && _holderByType.Count > 0)
			{
				foreach (IExtensionHolder extensionHolder in _holderByType.Values)
				{
					extensionHolder.Extend(container);
				}
			}
		}

		public void Install<TExtension>()
				where TExtension : class, IExtension<TExtension>, new()
		{
			ExtensionHolder<TExtension> extensionHolder;
			if (_parent != null && _parent.TryGetExtension(out TExtension parentExtension))
			{
				extensionHolder = new ExtensionHolder<TExtension>(_container, parentExtension);
			}
			else
			{
				extensionHolder = new ExtensionHolder<TExtension>(_container);
			}
			Type extensionType = typeof(TExtension);
			if (_holderByType == null)
			{
				_holderByType = new Dictionary<Type, IExtensionHolder> {{extensionType, extensionHolder}};
			}
			else if (_holderByType.ContainsKey(extensionType))
			{
				throw new ArgumentException($"Extension with type '{extensionType.Name}' already installed.");
			}
			else
			{
				_holderByType.Add(extensionType, extensionHolder);
			}
		}

		private interface IExtensionHolder : IDisposable
		{
			object Extension { get; }

			void Extend(Container container);
		}

		private sealed class ExtensionHolder<TExtension> : IExtensionHolder
				where TExtension : class, IExtension<TExtension>, new()
		{
			private readonly TExtension _extension;

			public ExtensionHolder(Container container, TExtension parent = null)
			{
				_extension = new TExtension();
				_extension.Extend(container, parent);
			}

			~ExtensionHolder()
			{
				Dispose();
			}

			public void Dispose()
			{
				if (_extension is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}

			public TExtension Extension => _extension;
			object IExtensionHolder.Extension => _extension;

			public void Extend(Container container)
			{
				container.Extend<TExtension>();
			}
		}
	}
}