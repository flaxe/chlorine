using System;

namespace Chlorine
{
	public class InjectException : Exception
	{
		public InjectException(string message) : base(message)
		{
		}

		public InjectException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}