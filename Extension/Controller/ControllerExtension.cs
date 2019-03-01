using Chlorine.Binder;

namespace Chlorine
{
	public class ControllerExtension : IExtension
	{
		public void Extend(Container container)
		{
			ControllerBinder binder = new ControllerBinder(container.TryResolve<ControllerBinder>());
			container.Bind<ControllerBinder>().ToInstance(binder);
			container.Bind<IController>().ToInstance(new Controller(binder));
		}
	}
}