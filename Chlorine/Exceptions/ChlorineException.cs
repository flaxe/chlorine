using System;

namespace Chlorine.Exceptions
{
	public enum ChlorineErrorCode
	{
		NotResetBeforeReuse = -0xFF001,

		InvalidOperation = -0xFFF01,
		InvalidState = -0xFFF02
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

		protected internal ChlorineException(int code, string message) :
				base(message)
		{
			HResult = code;
		}

		protected internal ChlorineException(int code, string message, Exception innerException) :
				base(message, innerException)
		{
			HResult = code;
		}
	}
}