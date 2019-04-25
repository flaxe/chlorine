using System;

namespace Chlorine
{
	public class ContainerException : Exception
	{
		public ContainerException(string message) : base(message)
		{
		}

		public ContainerException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}