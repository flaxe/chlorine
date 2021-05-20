using System;

namespace Carbone
{
	public readonly struct TypeValue
	{
		public readonly Type Type;
		public readonly object Value;

		public TypeValue(Type type, object value)
		{
			Type = type;
			Value = value;
		}
	}
}