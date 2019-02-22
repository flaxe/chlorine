using System;

namespace Chlorine
{
	public class InjectException : Exception
	{
		public InjectException(string message) : base(message)
		{
		}
	}

	public class ContainerException : Exception
	{
		public ContainerException(string message) : base(message)
		{
		}
	}
}