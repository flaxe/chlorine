namespace Chlorine
{
	internal class ConcreteProvider<TConcrete, T> : IProvider<T>
			where TConcrete : class, T
			where T : class
	{
		private readonly Container _container;

		public ConcreteProvider(Container container)
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