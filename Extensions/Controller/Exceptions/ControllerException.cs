using System;
using Chlorine.Exceptions;

namespace Chlorine.Controller.Exceptions
{
	public enum ControllerErrorCode
	{
		ActionNotRegistered = -0xC0001,
		ActionDoesNotReturnResult = -0xC0002,
		ActionAlreadyRegistered = -0xC0003,
		InvalidDelegate = -0xC0004,
		ExecutorNotRegistered = -0xC0005,
		ExecutorAlreadyRegistered = -0xC0006,

		IncompleteBinding = -0xC0101,
		UnexpectedBinding = -0xC0102,

		InitializationFailed = -0xC0201,
		ExecutionFailed = -0xC0202,
		GetResultFailed = -0xC0203,

		UnexpectedAction = -0xC0F01,
		InvalidType = -0xC0F02
	}

	public class ControllerException : ChlorineException
	{
		public ControllerException(ControllerErrorCode code, string message) :
				base((int)code, message)
		{
		}

		public ControllerException(ControllerErrorCode code, string message, Exception innerException) :
				base((int)code, message, innerException)
		{
		}

		public ControllerException(Error error) :
				base(error)
		{
		}
	}
}