using System;
using Chlorine.Controller.Bindings;
using Chlorine.Extensions;

namespace Chlorine.Controller
{
	public sealed class ControllerExtension : IExtension<ControllerExtension>, IDisposable
	{
		private ControllerBinder _binder;

		internal ControllerBinder Binder => _binder;

		public void Dispose()
		{
			_binder.Dispose();
		}

		public void Extend(Container container, ControllerExtension parent)
		{
			_binder = new ControllerBinder(parent?._binder);
			container.Bind<IController>().ToInstance(new Controller(_binder));
		}
	}
}