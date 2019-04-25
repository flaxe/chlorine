using System;

namespace Chlorine
{
	public class PromiseException : Exception
	{
		public PromiseException(string message) : base(message)
		{
		}

		public PromiseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}