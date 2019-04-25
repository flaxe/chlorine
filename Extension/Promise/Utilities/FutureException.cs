using System;

namespace Chlorine
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