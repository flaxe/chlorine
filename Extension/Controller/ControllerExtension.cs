using Chlorine.Bindings;
using Chlorine.Extension;

namespace Chlorine
{
	public sealed class ControllerExtension : IExtension<ControllerExtension>
	{
		private ControllerBinder _binder;

		public void Extend(Container container, ControllerExtension parent)
		{
			_binder = new ControllerBinder(parent?._binder);
			container.Bind<ControllerBinder>().ToInstance(_binder);
			container.Bind<IController>().To<Controller>().AsSingleton();
		}
	}
}