namespace Chlorine
{
	public interface IExecutor<in TExecutable> where TExecutable : class, IExecutable
	{
		void Execute(TExecutable executable);
	}

	public interface IExecutable
	{
		void Execute(IExecutionHandler handler);
	}

	public interface IExecutionHandler
	{
	}
}