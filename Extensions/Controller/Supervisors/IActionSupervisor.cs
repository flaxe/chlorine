using Chlorine.Futures;

namespace Chlorine.Controller.Supervisors
{
	internal interface IActionSupervisor<TAction>
			where TAction : struct
	{
		Expected<IFuture> Perform(ref TAction action);
	}

	internal interface IActionSupervisor<TAction, TResult> : IActionSupervisor<TAction>
			where TAction : struct
	{
		new Expected<IFuture<TResult>> Perform(ref TAction action);
	}
}