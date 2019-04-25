using System;

namespace Chlorine
{
	public class ControllerException : Exception
	{
		public ControllerException(string message) : base(message)
		{
		}

		public ControllerException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}