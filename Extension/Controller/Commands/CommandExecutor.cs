using System;
using System.Collections.Generic;
using Chlorine.Exceptions;
using Chlorine.Execution;

namespace Chlorine.Commands
{
	internal sealed class CommandExecutor : IExecutor<Command>, IExecutionHandler
	{
		private readonly Dictionary<IExecutable, IExecutionHandler> _handlerByExecutable;

		public CommandExecutor()
		{
			_handlerByExecutable = new Dictionary<IExecutable, IExecutionHandler>();
		}

		public void Execute(Command command, IExecutionHandler handler)
		{
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command));
			}
			if (!command.IsPending || _handlerByExecutable.ContainsKey(command))
			{
				throw new CommandException(CommandErrorCode.AlreadyExecuted,
						$"Command '{command.GetType().Name}' already executed.");
			}
			_handlerByExecutable.Add(command, handler);
		}

		public void HandleComplete(IExecutable executable)
		{
			if (!_handlerByExecutable.TryGetValue(executable, out IExecutionHandler handler))
			{
				throw new CommandException(CommandErrorCode.UnexpectedCommand,
						$"Unexpected command '{executable.GetType().Name}'");
			}
			_handlerByExecutable.Remove(executable);
			handler.HandleComplete(executable);
		}
	}
}