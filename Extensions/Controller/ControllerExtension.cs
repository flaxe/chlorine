using Chlorine.Controller.Bindings;
using Chlorine.Extensions;

namespace Chlorine.Controller
{
	public sealed class ControllerExtension : IExtension<ControllerExtension>
	{
		internal ControllerBinder Binder;

		public void Extend(Container container, ControllerExtension parent)
		{
			Binder = new ControllerBinder(parent?.Binder);
			container.Bind<IController>().ToInstance(new Controller(Binder));
		}
	}
}