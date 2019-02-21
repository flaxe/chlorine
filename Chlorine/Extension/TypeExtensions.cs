using System;

namespace Chlorine
{
	public static class TypeExtensions
	{
		public static bool DerivesFromOrEqual(this Type self, Type type)
		{
			return self == type || type.IsAssignableFrom(self);
		}
	}
}