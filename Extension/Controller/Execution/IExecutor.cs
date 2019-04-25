namespace Chlorine.Execution
{
	public interface IExecutor<in TExecutable> where TExecutable : class, IExecutable
	{
		void Execute(TExecutable executable, IExecutionHandler handler);
	}
}