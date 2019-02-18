namespace Chlorine
{
	internal class InstanceProvider<T> : IBindingProvider
			where T : class
	{
		private readonly T _instance;

		public InstanceProvider(T instance)
		{
			_instance = instance;
		}

		public object Provide()
		{
			return _instance;
		}
	}
}