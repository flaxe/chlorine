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

	public interface IExecutable<out TResult> : IExecutable
	{
		TResult Result { get; }
	}
}