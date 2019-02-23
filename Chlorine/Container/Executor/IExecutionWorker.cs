namespace Chlorine
{
	internal interface IExecutionWorker
	{
		void Execute(IExecutable executable);
	}

	internal interface IExecutionWorker<in TExecutable> : IExecutionWorker
			where TExecutable : class, IExecutable
	{
		void Execute(TExecutable executable);
	}
}