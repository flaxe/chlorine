using System;

namespace Chlorine.Providers
{
	public sealed class TypeProvider : IProvider
	{
		private readonly Type _type;
		private readonly IContainer _container;

		public TypeProvider(Type type, IContainer container)
		{
			_type = type;
			_container = container;
		}

		public object Provide()
		{
			return _container.Instantiate(_type);
		}
	}
}