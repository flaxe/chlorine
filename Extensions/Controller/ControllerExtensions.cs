using Chlorine.Controller.Bindings;
using Chlorine.Controller.Execution;

namespace Chlorine.Controller
{
	public static class ControllerExtensions
	{
		public static BindingAction<TAction> BindAction<TAction>(this Container container)
				where TAction : struct
		{
			ControllerExtension extension = container.Get<ControllerExtension>();
			return new BindingAction<TAction>(container, extension.Binder);
		}

		public static BindingExecutable<TExecutable> BindExecutable<TExecutable>(this Container container)
				where TExecutable : class, IExecutable
		{
			ControllerExtension extension = container.Get<ControllerExtension>();
			return new BindingExecutable<TExecutable>(container, extension.Binder);
		}
	}
}