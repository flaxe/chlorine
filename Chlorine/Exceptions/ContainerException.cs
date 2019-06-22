using System;

namespace Chlorine.Exceptions
{
	public enum ContainerErrorCode
	{
		TypeNotRegistered = -0xA0001,
		TypeAlreadyRegistered = -0xA0002,
		ExtensionNotInstalled = -0xA0003,
		ExtensionAlreadyInstalled = -0xA0004,
		BindingsAlreadyCompleted = -0xA0005,

		IncompleteBinding = -0xA0101,
		UnexpectedBinding = -0xA0102,

		InvalidOperation = -0xA0F01
	}

	public class ContainerException : ChlorineException
	{
		public ContainerException(ContainerErrorCode code, string message) :
				base((int)code, message)
		{
		}

		public ContainerException(ContainerErrorCode code, string message, Exception innerException) :
				base((int)code, message, innerException)
		{
		}
	}
}