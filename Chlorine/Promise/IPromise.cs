namespace Chlorine
{
	public interface IPromise
	{
		bool IsResolved { get; }
		bool IsRejected { get; }

		Error Reason { get; }

		void Fulfill(Future future);
		void Revoke(Future future);
		void RevokeAll();
	}

	public interface IPromise<TResult> : IPromise
	{
		TResult Result { get; }

		void Fulfill(Future<TResult> future);
		void Revoke(Future<TResult> future);
	}
}