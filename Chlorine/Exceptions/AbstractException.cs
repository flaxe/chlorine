using System;

namespace Chlorine.Exceptions
{
	public abstract class AbstractException : Exception
	{
		protected AbstractException(int code, string message) :
				base(message)
		{
			HResult = code;
		}

		protected AbstractException(int code, string message, Exception innerException) :
				base(message, innerException)
		{
			HResult = code;
		}
	}
}