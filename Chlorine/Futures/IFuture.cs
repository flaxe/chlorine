namespace Chlorine
{
	public interface IFuture
	{
		bool IsResolved { get; }
		bool IsRejected { get; }

		Error Reason { get; }

		void Clear();

		void Then(FutureResolved resolved, FutureRejected rejected);
		void Catch(FutureRejected rejected);
	}

	public interface IFuture<TResult> : IFuture
	{
		TResult Result { get; }

		bool TryGetResult(out TResult result);

		void Then(FutureResolved<TResult> resolved, FutureRejected rejected);
	}
}