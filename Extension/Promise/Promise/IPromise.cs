namespace Chlorine
{
	public interface IPromise
	{
		bool IsResolved { get; }
		bool IsRejected { get; }

		Error Reason { get; }
		bool TryGetReason(out Error reason);

		void Fulfill(Future future);
		void Revoke(Future future);
		void RevokeAll();
	}

	public interface IPromise<TResult> : IPromise
	{
		TResult Result { get; }
		bool TryGetResult(out TResult result);

		void Fulfill(Future<TResult> future);
		void Revoke(Future<TResult> future);
	}
}