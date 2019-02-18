namespace Chlorine
{
	public abstract class Installer
	{
		private Container _container;

		internal void Install(Container container)
		{
			_container = container;
			InstallBindings();
		}

		protected abstract void InstallBindings();

		protected BindingWithId<T> Bind<T>() where T : class
		{
			return _container.Bind<T>();
		}
	}
}