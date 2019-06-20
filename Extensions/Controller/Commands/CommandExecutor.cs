using System;
using System.Collections.Generic;
using Chlorine.Controller.Exceptions;
using Chlorine.Controller.Execution;
using Chlorine.Controller.Queues;

namespace Chlorine.Controller.Commands
{
	internal sealed class CommandExecutor : IExecutor<ICommand>, IExecutionHandler
	{
		private readonly IExecutionQueue _queue;
		private readonly Dictionary<IExecutable, IExecutionHandler> _handlerByExecutable;

		public CommandExecutor(IExecutionQueue queue)
		{
			_queue = queue;
			_handlerByExecutable = new Dictionary<IExecutable, IExecutionHandler>();
		}

		public void Execute(ICommand command, IExecutionHandler handler)
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
			_queue.Enqueue(command);
			HandleExecute();
		}

		private void HandleExecute()
		{
			if (_queue.Peek() is ICommand command)
			{
				command.Execute(this);
			}
		}

		void IExecutionHandler.HandleComplete(IExecutable executable)
		{
			if (!_handlerByExecutable.TryGetValue(executable, out IExecutionHandler handler))
			{
				throw new CommandException(CommandErrorCode.UnexpectedCommand,
						$"Unexpected command '{executable.GetType().Name}'.");
			}
			_handlerByExecutable.Remove(executable);
			_queue.Dequeue(executable);
			handler.HandleComplete(executable);
			HandleExecute();
		}
	}
}