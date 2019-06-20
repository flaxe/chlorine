using Chlorine.Controller.Exceptions;
using Chlorine.Providers;

namespace Chlorine.Controller.Execution
{
	internal class ExecutionDelegate<TExecutable> : IExecutionDelegate
			where TExecutable : class, IExecutable
	{
		private readonly IProvider<IExecutor<TExecutable>> _provider;

		public ExecutionDelegate(IProvider<IExecutor<TExecutable>> provider)
		{
			_provider = provider;
		}

		public void Execute(IExecutable executable, IExecutionHandler handler)
		{
			if (executable is TExecutable instance)
			{
				_provider.Provide().Execute(instance, handler);
			}
			else
			{
				throw new ControllerException(ControllerErrorCode.InvalidType,
						$"Executable has invalid type, '{typeof(TExecutable).Name}' is expected.");
			}
		}
	}
}