using Chlorine.Controller.Execution;

namespace Chlorine.Controller
{
	public interface IActionDelegate<in TAction> :
			IExecutable
			where TAction : struct
	{
		bool Init(TAction action);
	}

	public interface IActionDelegate<in TAction, out TResult> :
			IActionDelegate<TAction>,
			IExecutable<TResult>
			where TAction : struct
	{
	}
}