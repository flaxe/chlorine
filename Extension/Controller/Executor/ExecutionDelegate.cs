using System;
using Chlorine.Provider;

namespace Chlorine.Executor
{
	internal class ExecutionDelegate<TExecutable> : IExecutionDelegate
			where TExecutable : class, IExecutable
	{
		private readonly IProvider<IExecutor<TExecutable>> _provider;

		public ExecutionDelegate(IProvider<IExecutor<TExecutable>> provider)
		{
			_provider = provider;
		}

		public void Execute(IExecutable executable, IExecuteHandler handler)
		{
			if (executable is TExecutable instance)
			{
				_provider.Provide().Execute(instance, handler);
			}
			else
			{
				throw new ArgumentException($"Executable has invalid type, {typeof(TExecutable).Name} is expected.");
			}
		}
	}
}