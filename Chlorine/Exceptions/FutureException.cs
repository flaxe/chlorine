using System;

namespace Chlorine.Exceptions
{
	public class FutureException : Exception
	{
		public FutureException(string message) : base(message)
		{
		}

		public FutureException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}