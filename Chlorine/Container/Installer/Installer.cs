namespace Chlorine
{
	public abstract class Installer : IInstaller
	{
		private Container _container;

		protected Installer()
		{
		}

		public void Install(Container container)
		{
			_container = container;
			InstallBindings();
		}

		protected abstract void InstallBindings();

		protected BindingType<T> Bind<T>() where T : class
		{
			return _container.Bind<T>();
		}

		protected BindingAction<TAction> BindAction<TAction>() where TAction : struct
		{
			return _container.BindAction<TAction>();
		}

		protected BindingExecutable<TExecutable> BindExecutable<TExecutable>() where TExecutable : class, IExecutable
		{
			return _container.BindExecutable<TExecutable>();
		}
	}
}