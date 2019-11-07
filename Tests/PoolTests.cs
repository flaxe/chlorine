using Chlorine.Futures;
using Chlorine.Pools;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class PoolTests
	{
		private static readonly uint Result = 5;
		private static readonly Error Reason = new Error(-100, "Test");

		[TestFixture]
		private class PromisePoolTests
		{
			[Test]
			public void ResolvedPromise_Release()
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
			public void ResolvedResultPromise_Release()
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
			public void RejectedPromise_Release()
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
			public void RejectedResultPromise_Release()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);
				PromisePool.Release(promise);

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				Assert.IsTrue(reusedPromise.IsPending);
				Assert.AreSame(promise, reusedPromise);
			}

			[Test]
			public void ReleasedPromise_Clear()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				PromisePool.Release(promise);
				PromisePool.Clear();

				Promise reusedPromise = PromisePool.Pull();
				Assert.AreNotSame(promise, reusedPromise);
			}

			[Test]
			public void ReleasedResultPromise_Clear()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				PromisePool.Release(promise);
				PromisePool.Clear();

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				Assert.AreNotSame(promise, reusedPromise);
			}
		}

		[TestFixture]
		private class FuturePoolTests
		{
			[Test]
			public void ResolvedFuture_Release()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Resolve();
				FuturePool.Release(future);

				Promise reusedPromise = PromisePool.Pull();
				IFuture reusedFuture = FuturePool.Pull(reusedPromise);
				Assert.IsTrue(reusedFuture.IsPending);
				Assert.AreSame(future, reusedFuture);
			}

			[Test]
			public void ResolvedResultFuture_Release()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Resolve(Result);
				FuturePool.Release(future);

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				IFuture<uint> reusedFuture = FuturePool.Pull(reusedPromise);
				Assert.IsTrue(reusedFuture.IsPending);
				Assert.AreSame(future, reusedFuture);
			}

			[Test]
			public void RejectedFuture_Release()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Reject(Reason);
				FuturePool.Release(future);

				Promise reusedPromise = PromisePool.Pull();
				IFuture reusedFuture = FuturePool.Pull(reusedPromise);
				Assert.IsTrue(reusedFuture.IsPending);
				Assert.AreSame(future, reusedFuture);
			}

			[Test]
			public void RejectedResultFuture_Release()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Reject(Reason);
				FuturePool.Release(future);

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				IFuture<uint> reusedFuture = FuturePool.Pull(reusedPromise);
				Assert.IsTrue(reusedFuture.IsPending);
				Assert.AreSame(future, reusedFuture);
			}

			[Test]
			public void ReleasedFuture_Clear()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				FuturePool.Release(future);
				FuturePool.Clear();

				Promise reusedPromise = PromisePool.Pull();
				IFuture reusedFuture = FuturePool.Pull(reusedPromise);
				Assert.AreNotSame(future, reusedFuture);
			}

			[Test]
			public void ReleasedResultFuture_Clear()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				FuturePool.Release(future);
				FuturePool.Clear();

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				IFuture<uint> reusedFuture = FuturePool.Pull(reusedPromise);
				Assert.AreNotSame(future, reusedFuture);
			}
		}
	}
}