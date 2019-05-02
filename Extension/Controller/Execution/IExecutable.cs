namespace Chlorine.Execution
{
	public interface IExecutable
	{
		bool IsPending { get; }
		bool IsSucceed { get; }
		bool IsFailed { get; }

		Error Error { get; }

		void Execute(IExecutionHandler handler);
	}
}