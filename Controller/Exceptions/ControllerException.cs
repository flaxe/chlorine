using System;

namespace Carbone.Exceptions
{
	public enum ControllerErrorCode
	{
		ActionNotRegistered = -0xC0001,
		ActionHasNoResult = -0xC0002,
		ActionAlreadyRegistered = -0xC0003,
		ExecutorNotRegistered = -0xC0004,
		ExecutorAlreadyRegistered = -0xC0005,

		IncompleteBinding = -0xC0101,
		UnexpectedBinding = -0xC0102,

		InitializationFailed = -0xC0201,
		ExecutionFailed = -0xC0202,
		GetResultFailed = -0xC0203,

		UnexpectedExecutable = -0xC0F01,
		InvalidType = -0xC0F02
	}

	public class ControllerException : AbstractException
	{
		public ControllerException(ControllerErrorCode code, string message) :
				base((int)code, message)
		{
		}

		public ControllerException(ControllerErrorCode code, string message, Exception innerException) :
				base((int)code, message, innerException)
		{
		}
	}
}