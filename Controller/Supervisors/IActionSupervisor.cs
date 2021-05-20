using Carbone.Futures;

namespace Carbone.Supervisors
{
	internal interface IActionSupervisor<TAction>
			where TAction : struct
	{
		Expected<IFuture> Perform(in TAction action);
	}

	internal interface IActionSupervisor<TAction, TResult> : IActionSupervisor<TAction>
			where TAction : struct
	{
		new Expected<IFuture<TResult>> Perform(in TAction action);
	}
}