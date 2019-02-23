namespace Chlorine
{
	internal class FromSubContainerProvider<TInstaller, T> : IProvider<T>
			where TInstaller : class, IInstaller
			where T : class
	{
		private readonly Container _container;
		private readonly object _id;

		private Container _subContainer;

		public FromSubContainerProvider(Container container, object id = null)
		{
			_container = container;
			_id = id;
		}

		public T Provide()
		{
			if (_subContainer == null)
			{
				_subContainer = new Container(_container);
				_subContainer.Install<TInstaller>();
			}
			return _subContainer.Resolve<T>(_id);
		}

		object IProvider.Provide()
		{
			return Provide();
		}
	}
}