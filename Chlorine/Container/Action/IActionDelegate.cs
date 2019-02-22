namespace Chlorine
{
	public interface IActionDelegate<in TAction>
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