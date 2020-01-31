using System;
using Chlorine.Exceptions;

namespace Chlorine.Providers
{
	public sealed class TypeProvider : IProvider
	{
		private readonly Type _type;
		private readonly IContainer _container;

		public TypeProvider(Type type, IContainer container)
		{
			if (type.IsInterface || type.IsAbstract)
			{
				throw new InvalidArgumentException(InvalidArgumentErrorCode.InvalidType,
						$"Type '{type.Name}' is abstract.");
			}
			_type = type;
			_container = container;
		}

		public object Provide()
		{
			return _container.Instantiate(_type);
		}
	}
}