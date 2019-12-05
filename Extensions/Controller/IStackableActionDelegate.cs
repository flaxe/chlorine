using Chlorine.Controller.Execution;

namespace Chlorine.Controller
{
	public interface IStackableActionDelegate<in TAction> :
			IActionDelegate<TAction>,
			IStackable<TAction>,
			IExecutable
			where TAction : struct
	{
	}

	public interface IStackableActionDelegate<in TAction, TResult> :
			IActionDelegate<TAction, TResult>,
			IStackable<TAction>,
			IExecutable<TResult>
			where TAction : struct
	{
	}
}