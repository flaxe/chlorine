namespace Chlorine.Controller
{
	public static class ContainerExtensions
	{
		public static BindingAction<TAction> BindAction<TAction>(this Container container)
				where TAction : struct
		{
			return new BindingAction<TAction>(container, container.Resolve<ControllerBinder>());
		}

		public static BindingExecutable<TExecutable> BindExecutable<TExecutable>(this Container container)
				where TExecutable : class, IExecutable
		{
			return new BindingExecutable<TExecutable>(container, container.Resolve<ControllerBinder>());
		}
	}
}