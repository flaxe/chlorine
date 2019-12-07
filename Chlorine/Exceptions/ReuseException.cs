using System;

namespace Chlorine.Exceptions
{
	public class ReuseException : ForbiddenOperationException
	{
		public ReuseException(object instance) :
				base(ForbiddenOperationErrorCode.AlreadyInitialized,
						$"Instance '{instance.GetType().Name}' already initialized. Try reset before reuse.")
		{
		}

		public ReuseException(object instance, Exception innerException) :
				base(ForbiddenOperationErrorCode.AlreadyInitialized,
						$"Instance '{instance.GetType().Name}' already initialized. Try reset before reuse.",
						innerException)
		{
		}
	}
}