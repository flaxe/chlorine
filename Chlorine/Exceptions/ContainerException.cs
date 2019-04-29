using System;

namespace Chlorine.Exceptions
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