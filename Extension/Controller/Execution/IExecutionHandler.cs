namespace Chlorine.Execution
{
	public interface IExecutionHandler
	{
		void HandleComplete(IExecutable executable);
	}
}