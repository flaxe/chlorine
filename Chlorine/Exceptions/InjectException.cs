using System;

namespace Chlorine.Exceptions
{
	public enum InjectErrorCode
	{
		TypeNotSupported = -0xAD001,
		TypeNotRegistered = -0xAD002,

		HasNoConstructor = -0xAD101,
		MultipleConstructors = -0xAD102,
		MultipleAttributes = -0xAD103,

		AbstractClassConstruction = -0xAD201,
		UnityComponentConstruction = -0xAD202,

		ReadonlyProperty = -0xAD301,

		InvalidType = -0xADF01
	}

	public class InjectException : ChlorineException
	{
		public InjectException(InjectErrorCode code, string message) :
				base((int)code, message)
		{
		}

		public InjectException(InjectErrorCode code, string message, Exception innerException) :
				base((int)code, message, innerException)
		{
		}
	}
}