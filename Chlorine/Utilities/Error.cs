using System;
using System.Text;

namespace Chlorine
{
	public struct Error
	{
		private readonly int _code;
		private readonly string _message;
		private readonly Exception _exception;

		public Error(string message)
		{
			_code = -1;
			_message = message;
			_exception = null;
		}

		public Error(int code, string message)
		{
			_code = code;
			_message = message;
			_exception = null;
		}

		public Error(int code, string message, Exception exception)
		{
			_code = code;
			_message = message;
			_exception = exception;
		}

		public Error(Exception exception)
		{
			_code = exception.HResult;
			_message = null;
			_exception = exception;
		}

		public int Code => _code;
		public string Message => _message;
		public Exception Exception => _exception;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (!string.IsNullOrEmpty(_message))
			{
				builder.Append(_message);
				builder.Append('(').Append(_code.ToString()).Append(')');
			}
			if (_exception != null)
			{
				if (builder.Length > 0)
				{
					builder.Append(Environment.NewLine);
				}
				builder.Append(_exception.Message);
				builder.Append(Environment.NewLine);
				builder.Append(_exception.StackTrace);
			}
			return builder.ToString();
		}
	}
}