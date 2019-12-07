using System;

namespace Chlorine.Exceptions
{
	public enum InvalidArgumentErrorCode
	{
		UnexpectedArgument = -0xFE001
	}

	public class InvalidArgumentException : AbstractException
	{
		public InvalidArgumentException(InvalidArgumentErrorCode code, string message) :
				base((int)code, message)
		{
		}

		public InvalidArgumentException(InvalidArgumentErrorCode code, string message, Exception innerException) :
				base((int)code, message, innerException)
		{
		}
	}
}