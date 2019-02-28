namespace Chlorine.Controller
{
	public class ControllerExtension : IContainerExtension
	{
		public void Extend(Container container)
		{
			ControllerBinder binder = new ControllerBinder(container.TryResolve<ControllerBinder>());
			Controller controller = new Controller(binder);

			container.Bind<ControllerBinder>().ToInstance(binder);
			container.Bind<IController>().ToInstance(controller);
		}
	}
}