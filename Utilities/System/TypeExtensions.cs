namespace System
{
	public static class TypeExtensions
	{
		public static bool IsEqualOrDerivesFrom(this Type self, Type type)
		{
			return self == type || type.IsAssignableFrom(self);
		}
	}
}