using System;

namespace Chlorine
{
	public struct Error
	{
		private readonly int _code;
		private readonly string _message;

		public Error(int code, string message)
		{
			_code = code;
			_message = message;
		}

		public Error(string message)
		{
			_code = -1;
			_message = message;
		}

		public Error(int code, Exception exception)
		{
			_code = code;
			_message = exception.Message;
		}

		public Error(Exception exception)
		{
			_code = -1;
			_message = exception.Message;
		}

		public int Code => _code;
		public string Message => _message;

		public override string ToString()
		{
			return $"{_message}({_code.ToString()})";
		}
	}
}