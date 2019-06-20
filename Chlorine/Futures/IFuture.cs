namespace Chlorine.Futures
{
	public interface IFuture
	{
		bool IsResolved { get; }
		bool IsRejected { get; }

		Error Reason { get; }

		void Clear();

		IFuture Then(FuturePromised promised);
		IFuture<T> Then<T>(FutureResultPromised<T> promised);
		void Then(FutureResolved resolved, FutureRejected rejected);
		void Catch(FutureRejected rejected);
		void Finally(IFutureHandler handler);
	}

	public interface IFuture<TResult> : IFuture
	{
		TResult Result { get; }
		bool TryGetResult(out TResult result);

		IFuture Then(FuturePromised<TResult> promised);
		IFuture<T> Then<T>(FutureResultPromised<T, TResult> promised);
		void Then(FutureResolved<TResult> resolved, FutureRejected rejected);
		void Finally(IFutureHandler<TResult> handler);
	}
}