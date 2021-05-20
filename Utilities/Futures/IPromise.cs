namespace Carbone.Futures
{
	public interface IPromise
	{
		bool IsPending { get; }
		bool IsResolved { get; }
		bool IsRejected { get; }

		Error Reason { get; }

		void Fulfill(Future future);
		void Revoke(Future future);
	}

	public interface IPromise<TResult> : IPromise
	{
		TResult Result { get; }
		bool TryGetResult(out TResult result);

		void Fulfill(Future<TResult> future);
		void Revoke(Future<TResult> future);
	}
}