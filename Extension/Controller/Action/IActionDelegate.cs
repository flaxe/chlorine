using Chlorine.Executor;

namespace Chlorine
{
	public interface IActionDelegate<in TAction> : IExecutable
			where TAction : struct
	{
		bool Init(TAction action);
	}

	public interface IActionDelegate<in TAction, TResult> : IActionDelegate<TAction>
			where TAction : struct
	{
		bool TryGetResult(out TResult result);
	}
}