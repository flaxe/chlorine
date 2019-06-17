using System;

namespace Chlorine.Exceptions
{
	public class ReuseException : ChlorineException
	{
		public ReuseException(string objectName) :
				base(ChlorineErrorCode.NotResetBeforeReuse, $"{objectName} was not reset before reuse.")
		{
		}

		public ReuseException(string objectName, Exception innerException) :
				base(ChlorineErrorCode.NotResetBeforeReuse, $"{objectName} was not reset before reuse.", innerException)
		{
		}
	}
}