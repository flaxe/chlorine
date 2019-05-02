using System;
using Chlorine.Bindings;
using Chlorine.Collections;
using Chlorine.Exceptions;
using Chlorine.Extensions;
using Chlorine.Injection;
using Chlorine.Providers;

namespace Chlorine
{
	public sealed class Container : IContainer, IDisposable
	{
		private readonly Container _parent;
		private WeakReferenceList<Container> _children;

		private readonly Binder _binder;
		private readonly Extender _extender;
		private readonly Injector _injector;

		public Container()
		{
			_binder = new Binder(this);
			_extender = new Extender(this);
			_injector = new Injector(new InjectAnalyzer(), _binder);

			_binder.Bind(new InstanceProvider<IContainer>(this));
		}

		private Container(Container parent)
		{
			_parent = parent;
			_binder = new Binder(this, _parent._binder);
			_extender = new Extender(this, _parent._extender);
			_injector = new Injector(_parent._injector.Analyzer, _binder);

			_binder.Bind(new InstanceProvider<IContainer>(this));
		}

		~Container()
		{
			Dispose();
		}

		public Container Parent => _parent;

		public void Dispose()
		{
			_binder.Dispose();
			_extender.Dispose();
			if (_children != null)
			{
				foreach (Container child in _children)
				{
					child.Dispose();
				}
				_children = null;
			}
		}

		public TExtension Get<TExtension>() where TExtension : class, IExtension<TExtension>, new()
		{
			if (_extender.TryGetExtension(out TExtension extension))
			{
				return extension;
			}
			throw new ContainerException(ContainerErrorCode.ExtensionNotInstalled,
					$"Extension with type '{typeof(TExtension).Name}' not installed.");
		}

		public Container CreateSubContainer()
		{
			Container container = new Container(this);
			if (_children == null)
			{
				_children = new WeakReferenceList<Container> {container};
			}
			else
			{
				_children.Add(container);
			}
			_extender.Extend(container);
			return container;
		}

		public void Extend<TExtension>() where TExtension : class, IExtension<TExtension>, new()
		{
			_extender.Install<TExtension>();
			if (_children != null && _children.Count > 0)
			{
				foreach (Container child in _children)
				{
					child.Extend<TExtension>();
				}
			}
		}

		public void Install<TInstaller>(TypeValue[] arguments = null) where TInstaller : class, IInstaller
		{
			Install(Instantiate<TInstaller>(arguments));
		}

		public void Install(Type type, TypeValue[] arguments = null)
		{
			IInstaller installer = Instantiate(type, arguments) as IInstaller;
			if (installer == null)
			{
				throw new ContainerException(ContainerErrorCode.InvalidType,
						$"Invalid type '{type.Name}' given for installer.");
			}
			Install(installer);
		}

		public void Install(IInstaller installer)
		{
			if (installer == null)
			{
				throw new ArgumentNullException(nameof(installer));
			}
			installer.Install(this);
		}

		public BindingType<T> Bind<T>() where T : class
		{
			return _binder.Bind<T>();
		}

		public T Resolve<T>(object id = null) where T : class
		{
			if (_binder.TryResolveType(id, out T instance))
			{
				return instance;
			}
			throw new ContainerException(ContainerErrorCode.TypeNotRegistered,
					$"Unable to resolve '{typeof(T).Name}'{(id != null ? $" with id {id}." : ".")}");
		}

		public object Resolve(Type type, object id = null)
		{
			if (_binder.TryResolveType(type, id, out object instance))
			{
				return instance;
			}
			throw new ContainerException(ContainerErrorCode.TypeNotRegistered,
					$"Unable to resolve '{type.Name}'{(id != null ? $" with id {id}." : ".")}");
		}

		public T TryResolve<T>(object id = null) where T : class
		{
			return _binder.TryResolveType(id, out T instance) ? instance : default;
		}

		public object TryResolve(Type type, object id = null)
		{
			return _binder.TryResolveType(type, id, out object instance) ? instance : default;
		}

		public T Instantiate<T>(TypeValue[] arguments = null)
		{
			return (T)_injector.Instantiate(typeof(T), arguments);
		}

		public object Instantiate(Type type, TypeValue[] arguments = null)
		{
			return _injector.Instantiate(type, arguments);
		}

		public void Inject(object instance, TypeValue[] arguments = null)
		{
			_injector.Inject(instance, arguments);
		}
	}
}