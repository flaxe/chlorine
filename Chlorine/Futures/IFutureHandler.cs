namespace Chlorine
{
	public interface IFutureHandler
	{
		void HandleFuture(IFuture future);
	}

	public interface IFutureHandler<TResult>
	{
		void HandleFuture(IFuture<TResult> future);
	}
}