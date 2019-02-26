using System;

namespace Chlorine
{
	public class Container : IContainer
	{
		private readonly Binder _binder;
		private readonly Injector _injector;

		public Container(Container parentContainer = null)
		{
			_binder = new Binder(parentContainer?._binder);
			_injector = new Injector(_binder);

			_binder.Bind(new InstanceProvider<IContainer>(this));
			_binder.Bind(new InstanceProvider<IController>(new Controller(_binder)));

			if (parentContainer == null)
			{
				_binder.Bind(new InstanceProvider<InjectAnalyzer>(new InjectAnalyzer()));
			}
		}

		public void Install<TInstaller>(TypeValue[] arguments = null) where TInstaller : class, IInstaller
		{
			Instantiate<TInstaller>(arguments).Install(this);
		}

		public void Install(IInstaller installer)
		{
			installer.Install(this);
		}

		public BindingType<T> Bind<T>() where T : class
		{
			return new BindingType<T>(this, _binder);
		}

		public BindingAction<TAction> BindAction<TAction>() where TAction : struct
		{
			return new BindingAction<TAction>(this, _binder);
		}

		public BindingExecutable<TExecutable> BindExecutable<TExecutable>() where TExecutable : class, IExecutable
		{
			return new BindingExecutable<TExecutable>(this, _binder);
		}

		public T Resolve<T>(object id = null) where T : class
		{
			if (_binder.TryResolveType(id, out T instance))
			{
				return instance;
			}
			throw new ContainerException($"Unable to resolve '{typeof(T).Name}'{(id != null ? $" with id {id}." : ".")}");
		}

		public object Resolve(Type type, object id = null)
		{
			if (_binder.TryResolveType(type, id, out object instance))
			{
				return instance;
			}
			throw new ContainerException($"Unable to resolve '{type.Name}'{(id != null ? $" with id {id}." : ".")}");
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