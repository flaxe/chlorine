namespace Chlorine.Execution
{
	public interface IExecutable
	{
		bool IsSucceed { get; }
		bool IsFailed { get; }

		bool TryGetError(out Error error);

		void Execute(IExecutionHandler handler);
	}
}