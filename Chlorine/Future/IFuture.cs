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

	public interface IFuture<out TResult> : IFuture
	{
		TResult Result { get; }

		void Then(FutureResolved<TResult> resolved, FutureRejected rejected);
	}
}