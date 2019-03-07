using System;

namespace Chlorine
{
	public struct Error
	{
		public readonly int Code;
		public readonly string Message;

		public readonly Exception Exception;

		public Error(int code, string message)
		{
			Code = code;
			Message = message;
			Exception = null;
		}

		public Error(string message)
		{
			Code = -1;
			Message = message;
			Exception = null;
		}

		public Error(Exception exception)
		{
			Code = exception.HResult;
			Message = exception.Message;
			Exception = exception;
		}

		public override string ToString()
		{
			return $"{Message}({Code.ToString()}){(Exception != null ? $".{Environment.NewLine}{Exception.StackTrace}" : ".")}";
		}

		public static explicit operator Exception(Error error)
		{
			return error.Exception ?? new Exception(error.Message);
		}
	}
}