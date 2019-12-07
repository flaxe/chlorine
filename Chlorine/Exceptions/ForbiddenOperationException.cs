using System;

namespace Chlorine.Exceptions
{
	public enum ForbiddenOperationErrorCode
	{
		AlreadyInitialized = -0xFF001,

		InvalidOperation = -0xFFF01,
	}

	public class ForbiddenOperationException : AbstractException
	{
		public ForbiddenOperationException(ForbiddenOperationErrorCode code, string message) :
				base((int)code, message)
		{
		}

		public ForbiddenOperationException(ForbiddenOperationErrorCode code, string message, Exception innerException) :
				base((int)code, message, innerException)
		{
		}
	}
}