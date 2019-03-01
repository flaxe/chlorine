namespace Chlorine
{
	public class InstanceProvider<T> : IProvider<T>
			where T : class
	{
		private readonly T _instance;

		public InstanceProvider(T instance)
		{
			_instance = instance;
		}

		public T Provide()
		{
			return _instance;
		}

		object IProvider.Provide()
		{
			return Provide();
		}
	}
}