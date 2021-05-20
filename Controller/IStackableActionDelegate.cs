using Carbone.Execution;

namespace Carbone
{
	public interface IStackableActionDelegate<TAction> :
			IActionDelegate<TAction>,
			IStackable<TAction>,
			IExecutable
			where TAction : struct
	{
	}

	public interface IStackableActionDelegate<TAction, out TResult> :
			IActionDelegate<TAction, TResult>,
			IStackable<TAction>,
			IExecutable<TResult>
			where TAction : struct
	{
	}
}