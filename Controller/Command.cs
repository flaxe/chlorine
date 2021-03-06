using System;
using Carbone.Exceptions;
using Carbone.Execution;

namespace Carbone
{
	public abstract class Command : ICommand, IPoolable
	{
		protected enum ExecutionStatus
		{
			Pending,
			Succeed,
			Failed
		}

		private enum CommandStatus
		{
			Pending,
			Canceled,
			Executed,
			Succeed,
			Failed
		}

		private IExecutionHandler? _handler;

		private CommandStatus _status = CommandStatus.Pending;
		private Error _error;

		public bool IsPending => _status == CommandStatus.Pending;

		public bool IsSucceed => _status == CommandStatus.Succeed;
		public bool IsFailed => _status == CommandStatus.Failed;

		public Error Error
		{
			get
			{
				if (_status != CommandStatus.Failed)
				{
					throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
							$"Command \"{GetType().Name}\" was not failed.");
				}
				return _error;
			}
		}

		public void Reset()
		{
			HandleReset();
			_handler = null;
			_status = CommandStatus.Pending;
			_error = default;
		}

		public void Execute(IExecutionHandler handler)
		{
			if (_status != CommandStatus.Pending || _handler != null)
			{
				throw new ReuseException(this);
			}

			_handler = handler ?? throw new ArgumentNullException(nameof(handler));

			CheckResult check = HandleCheck();
			if (check.IsSucceed)
			{
				ExecutionResult execution = HandleExecute();
				_status = CommandStatus.Executed;

				switch (execution.Status)
				{
					case ExecutionStatus.Succeed:
						Apply();
						break;
					case ExecutionStatus.Failed:
						Fail(execution.Error);
						break;
				}
			}
			else
			{
				Cancel(check.Error);
			}
		}

		protected void Cancel(Error error)
		{
			if (_status != CommandStatus.Pending)
			{
				throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
						$"Can not cancel not pending command \"{GetType().Name}\".");
			}
			_status = CommandStatus.Canceled;
			_error = error;
			_handler?.HandleExecutable(this);
		}

		protected void Fail(Error error)
		{
			if (_status != CommandStatus.Executed)
			{
				throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
						$"Can not fail not executed command \"{GetType().Name}\".");
			}
			_status = CommandStatus.Failed;
			_error = error;
			HandleFault(in _error);
			_handler?.HandleExecutable(this);
		}

		protected void Apply()
		{
			if (_status != CommandStatus.Executed)
			{
				throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
						$"Can not apply not executed command \"{GetType().Name}\".");
			}
			_status = CommandStatus.Succeed;
			HandleApply();
			_handler?.HandleExecutable(this);
		}

		protected virtual void HandleReset()
		{
		}

		protected virtual CheckResult HandleCheck()
		{
			return true;
		}

		protected abstract ExecutionResult HandleExecute();

		protected virtual void HandleFault(in Error error)
		{
		}

		protected virtual void HandleApply()
		{
		}

		protected readonly struct CheckResult
		{
			private readonly bool _succeed;
			private readonly Error _error;

			private CheckResult(bool value)
			{
				_succeed = value;
				_error = _succeed == false ?
						new Error((int)CommandErrorCode.CheckFailed, "Check failed.") : default;
			}

			private CheckResult(in Error error)
			{
				_succeed = false;
				_error = error;
			}

			public bool IsSucceed => _succeed;

			public Error Error
			{
				get
				{
					if (_succeed)
					{
						throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
								"Check result has no error.");
					}
					return _error;
				}
			}

			public static implicit operator CheckResult(bool value)
			{
				return new CheckResult(value);
			}

			public static implicit operator CheckResult(Error error)
			{
				return new CheckResult(error);
			}
		}

		protected readonly struct ExecutionResult
		{
			private readonly ExecutionStatus _status;
			private readonly Error _error;

			private ExecutionResult(ExecutionStatus status)
			{
				_status = status;
				_error = _status == ExecutionStatus.Failed ?
						new Error((int)CommandErrorCode.ExecutionFailed, "Execution failed.") : default;
			}

			private ExecutionResult(in Error error)
			{
				_status = ExecutionStatus.Failed;
				_error = error;
			}

			public ExecutionStatus Status => _status;

			public Error Error
			{
				get
				{
					if (_status != ExecutionStatus.Failed)
					{
						throw new ForbiddenOperationException(ForbiddenOperationErrorCode.InvalidOperation,
								"Execution result has no error.");
					}
					return _error;
				}
			}

			public static implicit operator ExecutionResult(ExecutionStatus status)
			{
				return new ExecutionResult(status);
			}

			public static implicit operator ExecutionResult(Error error)
			{
				return new ExecutionResult(error);
			}
		}
	}
}