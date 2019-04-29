namespace Chlorine.Execution
{
	public interface IExecutable
	{
		bool IsProcessed { get; }
		bool IsSucceed { get; }
		bool IsFailed { get; }

		Error Error { get; }

		void Execute(IExecutionHandler handler);
	}
}