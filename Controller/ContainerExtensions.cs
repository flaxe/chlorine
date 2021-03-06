using Carbone.Bindings;
using Carbone.Execution;

namespace Carbone
{
	public static class ContainerExtensions
	{
		public static BindingAction<TAction> BindAction<TAction>(this Container container)
				where TAction : struct
		{
			ControllerExtension extension = container.Get<ControllerExtension>();
			return extension.Binder!.BindAction<TAction>(container);
		}

		public static BindingActionResult<TAction, TResult> BindAction<TAction, TResult>(this Container container)
				where TAction : struct
		{
			ControllerExtension extension = container.Get<ControllerExtension>();
			return extension.Binder!.BindAction<TAction, TResult>(container);
		}

		public static BindingExecutable<TExecutable> BindExecutable<TExecutable>(this Container container)
				where TExecutable : class, IExecutable
		{
			ControllerExtension extension = container.Get<ControllerExtension>();
			return extension.Binder!.BindExecutable<TExecutable>(container);
		}
	}
}