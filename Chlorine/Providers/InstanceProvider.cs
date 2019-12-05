namespace Chlorine.Providers
{
	public sealed class InstanceProvider : IProvider
	{
		private readonly object _instance;

		public InstanceProvider(object instance)
		{
			_instance = instance;
		}

		public object Provide()
		{
			return _instance;
		}
	}
}