using Carbone.Bindings;
using Carbone.Commands;
using Carbone.Execution;
using Carbone.Extensions;
using Carbone.Providers;

namespace Carbone
{
	public sealed class ControllerExtension : IExtension<ControllerExtension>
	{
		internal ControllerBinder? Binder;

		public void Extend(Container container, ControllerExtension? parent)
		{
			Binder = new ControllerBinder(parent?.Binder);
			if (parent == null)
			{
				Binder.RegisterExecutable(typeof(ICommand),
						new ExecutionDelegate<ICommand>(new InstanceProvider(new CommandExecutor())));
			}
			container.Bind<IController>().ToInstance(new Controller(Binder));
		}
	}
}