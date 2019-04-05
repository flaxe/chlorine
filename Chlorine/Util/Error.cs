using System;
using System.Text;

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
			StringBuilder builder = new StringBuilder();
			builder.Append(Message);
			builder.Append('(').Append(Code.ToString()).Append(')');
			if (Exception != null)
			{
				builder.Append(Environment.NewLine);
				builder.Append(Exception.StackTrace);
			}
			return builder.ToString();
		}

		public static explicit operator Exception(Error error)
		{
			return error.Exception ?? new Exception(error.Message);
		}
	}
}