namespace Chlorine.Provider
{
	public sealed class ConcreteProvider<TConcrete, T> : IProvider<T>
			where TConcrete : class, T
			where T : class
	{
		private readonly IContainer _container;

		public ConcreteProvider(IContainer container)
		{
			_container = container;
		}

		public T Provide()
		{
			return _container.Instantiate<TConcrete>();
		}

		object IProvider.Provide()
		{
			return Provide();
		}
	}
}