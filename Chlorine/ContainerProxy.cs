using System;
using Chlorine.Exceptions;

namespace Chlorine
{
	internal class ContainerProxy : IContainer
	{
		private readonly WeakReference<Container> _container;

		public ContainerProxy(Container container)
		{
			_container = new WeakReference<Container>(container);
		}

		public T Resolve<T>(object id = null) where T : class
		{
			if (_container.TryGetTarget(out Container container))
			{
				return container.Resolve<T>(id);
			}
			throw new ContainerException(ContainerErrorCode.InvalidOperation,
					"Invalid operation. Container was finalized.");
		}

		public object Resolve(Type type, object id = null)
		{
			if (_container.TryGetTarget(out Container container))
			{
				return container.Resolve(type, id);
			}
			throw new ContainerException(ContainerErrorCode.InvalidOperation,
					"Invalid operation. Container was finalized.");
		}

		public T TryResolve<T>(object id = null) where T : class
		{
			if (_container.TryGetTarget(out Container container))
			{
				return container.TryResolve<T>(id);
			}
			throw new ContainerException(ContainerErrorCode.InvalidOperation,
					"Invalid operation. Container was finalized.");
		}

		public object TryResolve(Type type, object id = null)
		{
			if (_container.TryGetTarget(out Container container))
			{
				return container.TryResolve(type, id);
			}
			throw new ContainerException(ContainerErrorCode.InvalidOperation,
					"Invalid operation. Container was finalized.");
		}

		public T Instantiate<T>(TypeValue[] arguments = null)
		{
			if (_container.TryGetTarget(out Container container))
			{
				return container.Instantiate<T>(arguments);
			}
			throw new ContainerException(ContainerErrorCode.InvalidOperation,
					"Invalid operation. Container was finalized.");
		}

		public object Instantiate(Type type, TypeValue[] arguments = null)
		{
			if (_container.TryGetTarget(out Container container))
			{
				return container.Instantiate(type, arguments);
			}
			throw new ContainerException(ContainerErrorCode.InvalidOperation,
					"Invalid operation. Container was finalized.");
		}

		public void Inject(object instance, TypeValue[] arguments = null)
		{
			if (_container.TryGetTarget(out Container container))
			{
				container.Inject(instance, arguments);
				return;
			}
			throw new ContainerException(ContainerErrorCode.InvalidOperation,
					"Invalid operation. Container was finalized.");
		}
	}
}