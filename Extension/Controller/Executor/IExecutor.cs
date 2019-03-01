namespace Chlorine.Executor
{
	public interface IExecutor<in TExecutable> where TExecutable : class, IExecutable
	{
		void Execute(TExecutable executable, IExecuteHandler handler);
	}
}