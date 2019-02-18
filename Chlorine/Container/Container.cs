using System;

namespace Chlorine
{
	public class Container : IContainer
	{
		private readonly Container _parentContainer;

		private readonly Injector _injector;
		private readonly Binder _binder;

		public Container(Container parentContainer = null)
		{
			_parentContainer = parentContainer;

			_injector = new Injector(this);

			_binder = new Binder();
			_binder.Bind(typeof(IContainer), null, new InstanceProvider<Container>(this));
		}

		public void Install<TInstaller>(object[] arguments = null) where TInstaller : Installer
		{
			Instantiate<TInstaller>(arguments).Install(this);
		}

		public void Install(Installer installer)
		{
			installer.Install(this);
		}

		public BindingWithId<T> Bind<T>() where T : class
		{
			return new BindingWithId<T>(_binder, this);
		}

		public T Resolve<T>(object id = null) where T : class
		{
			return (T)_binder.Resolve(typeof(T), id) ?? _parentContainer?.Resolve<T>(id);
		}

		public object Resolve(Type type, object id = null)
		{
			return _binder.Resolve(type, id) ?? _parentContainer?.Resolve(type, id);
		}

		public T Instantiate<T>(object[] arguments = null)
		{
			return (T)_injector.Instantiate(typeof(T), arguments);
		}

		public object Instantiate(Type type, object[] arguments = null)
		{
			return _injector.Instantiate(type, arguments);
		}

		public void Inject(object instance, object[] arguments = null)
		{
			_injector.Inject(instance, arguments);
		}
	}
}