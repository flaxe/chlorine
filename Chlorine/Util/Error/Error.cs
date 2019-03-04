using System;

namespace Chlorine
{
	public struct Error
	{
		public readonly int Code;
		public readonly string Message;

		public Error(int code, string message)
		{
			Code = code;
			Message = message;
		}

		public Error(string message)
		{
			Code = -1;
			Message = message;
		}

		public Error(Exception exception)
		{
			Code = exception.HResult;
			Message = exception.Message;
		}

		public override string ToString()
		{
			return $"{Message}({Code.ToString()})";
		}
	}
}