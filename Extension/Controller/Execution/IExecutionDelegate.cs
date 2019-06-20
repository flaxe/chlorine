namespace Chlorine.Controller.Execution
{
	internal interface IExecutionDelegate
	{
		void Execute(IExecutable executable, IExecutionHandler handler);
	}
}