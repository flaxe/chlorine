using System;
using Chlorine.Exceptions;

namespace Chlorine.Controller.Exceptions
{
	public enum CommandErrorCode
	{
		AlreadyExecuted = -0xCC001,

		CheckFailed = -0xCCC01,
		ExecutionFailed = -0xCCC02,

		UnexpectedCommand = -0xCCF01,
		InvalidOperation = -0xCCF02
	}

	public class CommandException : ChlorineException
	{
		public CommandException(CommandErrorCode code, string message) :
				base((int)code, message)
		{
		}

		public CommandException(CommandErrorCode code, string message, Exception innerException) :
				base((int)code, message, innerException)
		{
		}
	}
}