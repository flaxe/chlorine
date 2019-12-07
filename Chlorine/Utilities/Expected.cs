using Chlorine.Exceptions;

namespace Chlorine
{
	public struct Expected<T>
	{
		private readonly T _value;
		private readonly bool _hasValue;
		private readonly Error _error;

		public Expected(T value)
		{
			_value = value;
			_hasValue = true;
			_error = default;
		}

		public Expected(Error error)
		{
			_error = error;
			_hasValue = false;
			_value = default;
		}

		public bool HasValue => _hasValue;

		public T Value
		{
			get
			{
				if (!_hasValue)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Invalid operation. Expected has no value.");
				}
				return _value;
			}
		}

		public Error Error
		{
			get
			{
				if (_hasValue)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							"Invalid operation. Expected has no error.");
				}
				return _error;
			}
		}

		public bool TryGetValue(out T value)
		{
			if (_hasValue)
			{
				value = _value;
				return true;
			}
			value = default;
			return false;
		}
	}
}