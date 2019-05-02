using System;
using System.Text;
using Chlorine.Exceptions;

namespace Chlorine
{
	public struct Error
	{
		public readonly int Code;
		public readonly string Message;

		public readonly Exception Exception;

		public Error(string message)
		{
			Code = -1;
			Message = message;
			Exception = null;
		}

		public Error(int code, string message)
		{
			Code = code;
			Message = message;
			Exception = null;
		}

		public Error(int code, string message, Exception exception)
		{
			Code = code;
			Message = message;
			Exception = exception;
		}

		public Error(Exception exception)
		{
			Code = exception.HResult;
			Message = null;
			Exception = exception;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (!string.IsNullOrEmpty(Message))
			{
				builder.Append(Message);
				builder.Append('(').Append(Code.ToString()).Append(')');
			}
			if (Exception != null)
			{
				if (builder.Length > 0)
				{
					builder.Append(Environment.NewLine);
				}
				builder.Append(Exception.Message);
				builder.Append(Environment.NewLine);
				builder.Append(Exception.StackTrace);
			}
			return builder.ToString();
		}

		public static implicit operator Exception(Error error)
		{
			return error.Exception ?? new ChlorineException(error.Code, error.Message);
		}
	}
}