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

		protected BindingAction<T> BindAction<T>() where T : struct
		{
			return _container.BindAction<T>();
		}
	}
}