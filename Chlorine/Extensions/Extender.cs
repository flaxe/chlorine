using System;
using System.Collections.Generic;

namespace Chlorine.Extensions
{
	internal sealed class Extender : IDisposable
	{
		private readonly Container _container;
		private readonly Extender _parent;

		private readonly Dictionary<Type, IExtending> _extendingByType;

		public Extender(Container container, Extender parent = null)
		{
			_container = container;
			_parent = parent;
			_extendingByType = new Dictionary<Type, IExtending>();
		}

		~Extender()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_extendingByType.Count > 0)
			{
				foreach (IExtending extending in _extendingByType.Values)
				{
					extending.Dispose();
				}
			}
		}

		public bool TryGetExtension<TExtension>(out TExtension extension)
				where TExtension : class, IExtension<TExtension>, new()
		{
			Type extensionType = typeof(TExtension);
			if (_extendingByType.TryGetValue(extensionType, out IExtending extending))
			{
				if (extending.Extension is TExtension targetExtension)
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
			if (_extendingByType.Count > 0)
			{
				foreach (IExtending extending in _extendingByType.Values)
				{
					extending.Extend(container);
				}
			}
		}

		public void Install<TExtension>()
				where TExtension : class, IExtension<TExtension>, new()
		{
			Extending<TExtension> extending;
			if (_parent != null && _parent.TryGetExtension(out TExtension parentExtension))
			{
				extending = new Extending<TExtension>(_container, parentExtension);
			}
			else
			{
				extending = new Extending<TExtension>(_container);
			}
			Type extensionType = typeof(TExtension);
			if (_extendingByType.ContainsKey(extensionType))
			{
				throw new ArgumentException($"Extension with type '{extensionType.Name}' already installed.");
			}
			_extendingByType.Add(extensionType, extending);
		}

		private interface IExtending : IDisposable
		{
			object Extension { get; }

			void Extend(Container container);
		}

		private sealed class Extending<TExtension> : IExtending
				where TExtension : class, IExtension<TExtension>, new()
		{
			private readonly TExtension _extension;

			public Extending(Container container, TExtension parent = null)
			{
				_extension = new TExtension();
				_extension.Extend(container, parent);
			}

			~Extending()
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
			object IExtending.Extension => _extension;

			public void Extend(Container container)
			{
				container.Extend<TExtension>();
			}
		}
	}
}