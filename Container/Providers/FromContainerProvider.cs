using System;

namespace Carbone.Providers
{
	public sealed class FromContainerProvider : IProvider
	{
		private readonly IContainer _container;
		private readonly Type _type;
		private readonly object? _id;

		public FromContainerProvider(IContainer container, Type type, object? id = null)
		{
			_container = container;
			_type = type;
			_id = id;
		}

		public object Provide()
		{
			return _container.Resolve(_type, _id);
		}
	}
}