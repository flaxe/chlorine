namespace Chlorine
{
	public interface IController
	{
		IFuture Perform<TAction>(TAction action) where TAction : struct;
		IFuture<TResult> Perform<TAction, TResult>(TAction action) where TAction : struct;
	}
}