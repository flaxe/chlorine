using System;

namespace Chlorine
{
	public struct Argument
	{
		public readonly Type Type;
		public readonly object Value;

		public Argument(Type type, object value)
		{
			Type = type;
			Value = value;
		}
	}
}