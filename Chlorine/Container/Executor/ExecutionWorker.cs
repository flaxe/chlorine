using System;

namespace Chlorine
{
	internal class ExecutionWorker<TExecutable> : IExecutionWorker<TExecutable>
			where TExecutable : class, IExecutable
	{
		private readonly IProvider<IExecutor<TExecutable>> _provider;

		public ExecutionWorker(IProvider<IExecutor<TExecutable>> provider)
		{
			_provider = provider;
		}

		public void Execute(TExecutable executable)
		{
			_provider.Provide().Execute(executable);
		}

		public void Execute(IExecutable executable)
		{
			if (executable is TExecutable instance)
			{
				Execute(instance);
			}
			else
			{
				throw new ArgumentException($"Executable has invalid type, {typeof(TExecutable).Name} is expected.");
			}
		}
	}
}