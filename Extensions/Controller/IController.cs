using Chlorine.Futures;

namespace Chlorine.Controller
{
	public interface IController
	{
		IFuture Perform<TAction>(TAction action) where TAction : struct;
		IFuture<TResult> Perform<TAction, TResult>(TAction action) where TAction : struct;

		IFuture TryPerform<TAction>(TAction action) where TAction : struct;
		IFuture<TResult> TryPerform<TAction, TResult>(TAction action) where TAction : struct;
	}
}