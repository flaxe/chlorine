namespace Chlorine.Supervisors
{
	internal interface IActionSupervisor<TAction>
		where TAction : struct
	{
		Expected<IPromise> Perform(ref TAction action);
	}

	internal interface IActionSupervisor<TAction, TResult> : IActionSupervisor<TAction>
			where TAction : struct
	{
		new Expected<IPromise<TResult>> Perform(ref TAction action);
	}
}