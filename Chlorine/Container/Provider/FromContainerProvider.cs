namespace Chlorine
{
	internal class FromContainerProvider<T> : IProvider<T>
			where T : class
	{
		private readonly Container _container;
		private readonly object _id;

		public FromContainerProvider(Container container, object id = null)
		{
			_container = container;
			_id = id;
		}

		public T Provide()
		{
			return _container.Resolve<T>(_id);
		}

		object IProvider.Provide()
		{
			return Provide();
		}
	}
}