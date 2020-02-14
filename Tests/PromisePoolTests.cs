using Chlorine.Exceptions;
using Chlorine.Futures;
using Chlorine.Pools;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class PromisePoolTests
	{
		private static readonly uint Result = 5;
		private static readonly Error Reason = new Error(-100, "Test");

		[TestFixture]
		private class PullTests
		{
			[Test]
			public void PromisePull_IsPending()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();

				Assert.IsTrue(promise.IsPending);
			}

			[Test]
			public void ResultPromisePull_IsPending()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();

				Assert.IsTrue(promise.IsPending);
			}
		}

		[TestFixture]
		private class ReuseTests
		{
			[Test]
			public void ResolvedPromiseReuse_AreSame()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Resolve();
				PromisePool.Release(promise);

				Promise reusedPromise = PromisePool.Pull();
				Assert.IsTrue(reusedPromise.IsPending);
				Assert.AreSame(promise, reusedPromise);
			}

			[Test]
			public void ResolvedResultPromiseReuse_AreSame()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);
				PromisePool.Release(promise);

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				Assert.IsTrue(reusedPromise.IsPending);
				Assert.AreSame(promise, reusedPromise);
			}

			[Test]
			public void RejectedPromiseReuse_AreSame()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Reject(Reason);
				PromisePool.Release(promise);

				Promise reusedPromise = PromisePool.Pull();
				Assert.IsTrue(reusedPromise.IsPending);
				Assert.AreSame(promise, reusedPromise);
			}

			[Test]
			public void RejectedResultPromiseReuse_AreSame()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);
				PromisePool.Release(promise);

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				Assert.IsTrue(reusedPromise.IsPending);
				Assert.AreSame(promise, reusedPromise);
			}
		}

		[TestFixture]
		private class ReleaseTests
		{
			[Test]
			public void CheckStatusReleasedPromise_ExceptionThrown()
			{
				SharedPool.Clear();
				IPromise promise = PromisePool.Pull();
				PromisePool.Release(promise);

				Assert.Throws<ForbiddenOperationException>(() => { bool _ = promise.IsPending; });
				Assert.Throws<ForbiddenOperationException>(() => { bool _ = promise.IsResolved; });
				Assert.Throws<ForbiddenOperationException>(() => { bool _ = promise.IsRejected; });
			}

			[Test]
			public void CheckStatusReleasedResultPromise_ExceptionThrown()
			{
				SharedPool.Clear();
				IPromise<uint> promise = PromisePool.Pull<uint>();
				PromisePool.Release(promise);

				Assert.Throws<ForbiddenOperationException>(() => { bool _ = promise.IsPending; });
				Assert.Throws<ForbiddenOperationException>(() => { bool _ = promise.IsResolved; });
				Assert.Throws<ForbiddenOperationException>(() => { bool _ = promise.IsRejected; });
			}

			[Test]
			public void FulfillReleasedPromise_ExceptionThrown()
			{
				SharedPool.Clear();
				IPromise promise = PromisePool.Pull();
				PromisePool.Release(promise);

				Assert.Throws<ForbiddenOperationException>(() => FuturePool.Pull(promise));
			}

			[Test]
			public void FulfillReleasedResultPromise_ExceptionThrown()
			{
				SharedPool.Clear();
				IPromise<uint> promise = PromisePool.Pull<uint>();
				PromisePool.Release(promise);

				Assert.Throws<ForbiddenOperationException>(() => FuturePool.Pull(promise));
			}
		}

		[TestFixture]
		private class ClearTests
		{
			[Test]
			public void ClearedPromiseReuse_AreNotSame()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				PromisePool.Release(promise);
				PromisePool.Clear();

				Promise reusedPromise = PromisePool.Pull();
				Assert.AreNotSame(promise, reusedPromise);
			}

			[Test]
			public void ClearedResultPromiseReuse_AreNotSame()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				PromisePool.Release(promise);
				PromisePool.Clear();

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				Assert.AreNotSame(promise, reusedPromise);
			}
		}
	}
}