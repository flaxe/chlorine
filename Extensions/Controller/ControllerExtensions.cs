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
			return extension.Binder.BindAction<TAction>(container);
		}

		public static BindingExecutable<TExecutable> BindExecutable<TExecutable>(this Container container)
				where TExecutable : class, IExecutable
		{
			ControllerExtension extension = container.Get<ControllerExtension>();
			return extension.Binder.BindExecutable<TExecutable>(container);
		}
	}
}