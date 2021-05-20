using Carbone.Futures;

namespace Carbone
{
	public interface IController
	{
		IFuture Perform<TAction>(in TAction action) where TAction : struct;
		IFuture<TResult> Perform<TAction, TResult>(in TAction action) where TAction : struct;

		IFuture TryPerform<TAction>(in TAction action) where TAction : struct;
		IFuture<TResult> TryPerform<TAction, TResult>(in TAction action) where TAction : struct;
	}
}