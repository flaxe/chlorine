namespace Chlorine.Executor
{
	public interface IExecuteHandler
	{
		void HandleComplete(IExecutable executable);
	}
}