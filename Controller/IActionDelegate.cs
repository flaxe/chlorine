using Carbone.Execution;

namespace Carbone
{
	public interface IActionDelegate<TAction> :
			IExecutable
			where TAction : struct
	{
		void Init(in TAction action);
	}

	public interface IActionDelegate<TAction, out TResult> :
			IActionDelegate<TAction>,
			IExecutable<TResult>
			where TAction : struct
	{
	}
}