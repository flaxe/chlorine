using System;
using System.Text;

namespace Carbone
{
	public readonly struct Error
	{
		private readonly int _code;
		private readonly string? _message;
		private readonly Exception? _exception;

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
		public string Message => _message ?? (_exception != null ? _exception.Message : string.Empty);

		public Exception ToException()
		{
			return _exception ?? new Exception(_message);
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(_message))
			{
				builder.AppendLine(_message);
			}
			if (_exception != null)
			{
				builder.AppendLine(_exception.Message);
				string stackTrace = _exception.StackTrace;
				if (!string.IsNullOrWhiteSpace(stackTrace))
				{
					builder.Append(stackTrace);
				}
				if (_exception is AggregateException aggregateException)
				{
					foreach (Exception innerException in aggregateException.InnerExceptions)
					{
						builder.AppendLine(innerException.Message);
						string innerStackTrace = innerException.StackTrace;
						if (!string.IsNullOrWhiteSpace(innerStackTrace))
						{
							builder.Append(innerStackTrace);
						}
					}
				}
				else
				{
					Exception? innerException = _exception.InnerException;
					if (innerException != null)
					{
						builder.AppendLine(innerException.Message);
						string innerStackTrace = innerException.StackTrace;
						if (!string.IsNullOrWhiteSpace(innerStackTrace))
						{
							builder.Append(innerStackTrace);
						}
					}
				}
			}
			return builder.ToString();
		}
	}
}