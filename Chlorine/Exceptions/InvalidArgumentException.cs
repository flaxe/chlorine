using System;

namespace Chlorine.Exceptions
{
	public enum InvalidArgumentErrorCode
	{
		InvalidType = -0xFE001,
		UnexpectedArgument = -0xFE002
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