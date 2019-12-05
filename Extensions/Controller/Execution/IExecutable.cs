namespace Chlorine.Controller.Execution
{
	public interface IExecutable
	{
		bool IsPending { get; }
		bool IsSucceed { get; }
		bool IsFailed { get; }

		Error Error { get; }

		void Execute(IExecutionHandler handler);
	}

	public interface IExecutable<TResult> : IExecutable
	{
		bool TryGetResult(out TResult result);
	}
}