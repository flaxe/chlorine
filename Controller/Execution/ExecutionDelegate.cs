using System;
using Carbone.Exceptions;
using Carbone.Providers;

namespace Carbone.Execution
{
	internal sealed class ExecutionDelegate<TExecutable> :
			IExecutionDelegate,
			IDisposable
			where TExecutable : class, IExecutable
	{
		private readonly IProvider _provider;

		public ExecutionDelegate(IProvider provider)
		{
			_provider = provider;
		}

		~ExecutionDelegate()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_provider is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		public void Execute(IExecutable executable, IExecutionHandler handler)
		{
			object executor = _provider.Provide();
			if (executable is TExecutable concreteExecutable && executor is IExecutor<TExecutable> concreteExecutor)
			{
				concreteExecutor.Execute(concreteExecutable, handler);
			}
			else
			{
				throw new ControllerException(ControllerErrorCode.InvalidType,
						$"Executable has invalid type, \"{typeof(TExecutable).Name}\" is expected.");
			}
		}
	}
}