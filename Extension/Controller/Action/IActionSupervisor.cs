namespace Chlorine.Action
{
	internal interface IActionSupervisor<TAction>
		where TAction : struct
	{
		Expected<IPromise> TryPerform(ref TAction action);
	}

	internal interface IActionSupervisor<TAction, TResult> : IActionSupervisor<TAction>
			where TAction : struct
	{
		new Expected<IPromise<TResult>> TryPerform(ref TAction action);
	}
}