namespace Chlorine.Controller
{
	internal interface IExecutionDelegate
	{
		void Execute(IExecutable executable, IExecuteHandler handler);
	}
}