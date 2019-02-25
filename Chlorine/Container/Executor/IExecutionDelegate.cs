namespace Chlorine
{
	internal interface IExecutionDelegate
	{
		void Execute(IExecutable executable, IExecuteHandler handler);
	}
}