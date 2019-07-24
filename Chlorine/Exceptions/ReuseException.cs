using System;

namespace Chlorine.Exceptions
{
	public class ReuseException : ChlorineException
	{
		public ReuseException(object instance) :
				base(ChlorineErrorCode.NotResetBeforeReuse, 
						$"Instance '{instance.GetType().Name}' was not reset before reuse.")
		{
		}

		public ReuseException(object instance, Exception innerException) :
				base(ChlorineErrorCode.NotResetBeforeReuse, 
						$"Instance '{instance.GetType().Name}' was not reset before reuse.", innerException)
		{
		}
	}
}