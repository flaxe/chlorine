using System;

namespace Chlorine.Exceptions
{
	public enum ChlorineErrorCode
	{
		InvalidOperation = -0xFFF01
	}

	public class ChlorineException : Exception
	{
		public ChlorineException(ChlorineErrorCode code, string message) :
				this((int)code, message)
		{
		}

		public ChlorineException(ChlorineErrorCode code, string message, Exception innerException) :
				this((int)code, message, innerException)
		{
		}

		public ChlorineException(int code, string message) :
				base(message)
		{
			HResult = code;
		}

		public ChlorineException(int code, string message, Exception innerException) :
				base(message, innerException)
		{
			HResult = code;
		}
	}
}