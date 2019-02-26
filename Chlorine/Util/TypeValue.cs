using System;

namespace Chlorine
{
	public struct TypeValue
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