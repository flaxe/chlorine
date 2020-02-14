using System;

namespace Chlorine.Providers
{
	public sealed class InstanceProvider : IProvider, IDisposable
	{
		private readonly object _instance;

		public InstanceProvider(object instance)
		{
			_instance = instance;
		}

		~InstanceProvider()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_instance is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		public object Provide()
		{
			return _instance;
		}
	}
}