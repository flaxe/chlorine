namespace Chlorine.Controller.Execution
{
	public interface IExecutionHandler
	{
		void HandleComplete(IExecutable executable);
	}
}