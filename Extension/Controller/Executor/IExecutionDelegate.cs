namespace Chlorine.Executor
{
	internal interface IExecutionDelegate
	{
		void Execute(IExecutable executable, IExecuteHandler handler);
	}
}