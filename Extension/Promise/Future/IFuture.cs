namespace Chlorine
{
	public interface IFuture
	{
		bool IsResolved { get; }
		bool IsRejected { get; }

		void Clear();

		bool TryGetReason(out Error reason);

		void Then(FutureResolved resolved, FutureRejected rejected);
		void Catch(FutureRejected rejected);
	}

	public interface IFuture<TResult> : IFuture
	{
		bool TryGetResult(out TResult result);

		void Then(FutureResolved<TResult> resolved, FutureRejected rejected);
	}
}