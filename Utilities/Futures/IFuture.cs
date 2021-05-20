namespace Carbone.Futures
{
	public interface IFuture
	{
		bool IsPending { get; }
		bool IsResolved { get; }
		bool IsRejected { get; }

		Error Reason { get; }

		IFuture Then(FuturePromised promised);
		IFuture<T> Then<T>(FutureResultPromised<T> promised);
		void Then(FutureResolved resolved, FutureRejected rejected);
		void Catch(FutureRejected rejected);
		void Finally(IFutureHandler finalizer);
	}

	public interface IFuture<TResult> : IFuture
	{
		TResult Result { get; }
		bool TryGetResult(out TResult result);

		IFuture Then(FuturePromised<TResult> promised);
		IFuture<T> Then<T>(FutureResultPromised<T, TResult> promised);
		void Then(FutureResolved<TResult> resolved, FutureRejected rejected);
	}
}