using Chlorine.Exceptions;
using Chlorine.Futures;
using Chlorine.Pools;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class FuturePoolTests
	{
		private static readonly uint Result = 5;
		private static readonly Error Reason = new Error(-100, "Test");

		[TestFixture]
		private class PullTests
		{
			[Test]
			public void FuturePull_IsPending()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);

				Assert.IsTrue(future.IsPending);
			}

			[Test]
			public void ResultFuturePull_IsPending()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);

				Assert.IsTrue(future.IsPending);
			}

			[Test]
			public void ResolvedFuturePull_IsResolved()
			{
				SharedPool.Clear();
				IFuture future = FuturePool.PullResolved();

				Assert.IsTrue(future.IsResolved);
			}

			[Test]
			public void ResolvedResultFuturePull_IsResolved()
			{
				SharedPool.Clear();
				IFuture<uint> future = FuturePool.PullResolved(Result);

				Assert.IsTrue(future.IsResolved);
				Assert.AreEqual(Result, future.Result);
			}

			[Test]
			public void RejectedFuturePull_IsRejected()
			{
				SharedPool.Clear();
				IFuture future = FuturePool.PullRejected(Reason);

				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Reason, future.Reason);
			}

			[Test]
			public void RejectedResultFuturePull_IsRejected()
			{
				SharedPool.Clear();
				IFuture<uint> future = FuturePool.PullRejected<uint>(Reason);

				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Reason, future.Reason);
			}
		}

		[TestFixture]
		private class ReuseTests
		{
			[Test]
			public void ResolvedFutureReuse_AreSame()
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
			public void ResolvedResultFutureReuse_AreSame()
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
			public void RejectedFutureReuse_AreSame()
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
			public void RejectedResultFutureReuse_AreSame()
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
			public void PendingChainFutureToFutureReuse_AreSame()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture firstFuture = FuturePool.Pull(firstPromise);
				IFuture secondFuture = null;
				IFuture chainFuture = firstFuture.Then(() => secondFuture = FuturePool.Pull(secondPromise));
				firstPromise.Resolve();
				FuturePool.Release(chainFuture);

				for (int i = 0; i < 2; i++)
				{
					Promise reusedPromise = PromisePool.Pull();
					IFuture reusedFuture = FuturePool.Pull(reusedPromise);
					Assert.IsTrue(reusedFuture.IsPending);
					Assert.IsTrue(reusedFuture == firstFuture || reusedFuture == secondFuture);
				}
			}

			[Test]
			public void ResolvedChainFutureToFutureReuse_AreSame()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture firstFuture = FuturePool.Pull(firstPromise);
				IFuture secondFuture = null;
				IFuture chainFuture = firstFuture.Then(() => secondFuture = FuturePool.Pull(secondPromise));
				firstPromise.Resolve();
				secondPromise.Resolve();
				FuturePool.Release(chainFuture);

				for (int i = 0; i < 2; i++)
				{
					Promise reusedPromise = PromisePool.Pull();
					IFuture reusedFuture = FuturePool.Pull(reusedPromise);
					Assert.IsTrue(reusedFuture.IsPending);
					Assert.IsTrue(reusedFuture == firstFuture || reusedFuture == secondFuture);
				}
			}

			[Test]
			public void PendingChainResultFutureToResultFutureReuse_AreSame()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> firstFuture = FuturePool.Pull(firstPromise);
				IFuture<uint> secondFuture = null;
				IFuture<uint> chainFuture = firstFuture.Then(result => secondFuture = FuturePool.Pull(secondPromise));
				firstPromise.Resolve(Result);
				FuturePool.Release(chainFuture);

				for (int i = 0; i < 2; i++)
				{
					Promise<uint> reusedPromise = PromisePool.Pull<uint>();
					IFuture<uint> reusedFuture = FuturePool.Pull(reusedPromise);
					Assert.IsTrue(reusedFuture.IsPending);
					Assert.IsTrue(reusedFuture == firstFuture || reusedFuture == secondFuture);
				}
			}

			[Test]
			public void ResolvedChainResultFutureToResultFutureReuse_AreSame()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> firstFuture = FuturePool.Pull(firstPromise);
				IFuture<uint> secondFuture = null;
				IFuture<uint> chainFuture = firstFuture.Then(result => secondFuture = FuturePool.Pull(secondPromise));
				firstPromise.Resolve(Result);
				secondPromise.Resolve(Result);
				FuturePool.Release(chainFuture);

				for (int i = 0; i < 2; i++)
				{
					Promise<uint> reusedPromise = PromisePool.Pull<uint>();
					IFuture<uint> reusedFuture = FuturePool.Pull(reusedPromise);
					Assert.IsTrue(reusedFuture.IsPending);
					Assert.IsTrue(reusedFuture == firstFuture || reusedFuture == secondFuture);
				}
			}

			[Test]
			public void PendingFutureAllReuse_AreSame()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture firstFuture = FuturePool.Pull(firstPromise);
				IFuture secondFuture = FuturePool.Pull(secondPromise);
				IFuture futureAll = FuturePool.Pull(new[] {firstFuture, secondFuture});
				FuturePool.Release(futureAll);

				for (int i = 0; i < 2; i++)
				{
					Promise reusedPromise = PromisePool.Pull();
					IFuture reusedFuture = FuturePool.Pull(reusedPromise);
					Assert.IsTrue(reusedFuture.IsPending);
					Assert.IsTrue(reusedFuture == firstFuture || reusedFuture == secondFuture);
				}
			}

			[Test]
			public void ResolvedFutureAllReuse_AreSame()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture firstFuture = FuturePool.Pull(firstPromise);
				IFuture secondFuture = FuturePool.Pull(secondPromise);
				IFuture futureAll = FuturePool.Pull(new[] {firstFuture, secondFuture});
				firstPromise.Resolve();
				secondPromise.Resolve();
				FuturePool.Release(futureAll);

				for (int i = 0; i < 2; i++)
				{
					Promise reusedPromise = PromisePool.Pull();
					IFuture reusedFuture = FuturePool.Pull(reusedPromise);
					Assert.IsTrue(reusedFuture.IsPending);
					Assert.IsTrue(reusedFuture == firstFuture || reusedFuture == secondFuture);
				}
			}

			[Test]
			public void PendingResultFutureAllReuse_AreSame()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> firstFuture = FuturePool.Pull(firstPromise);
				IFuture<uint> secondFuture = FuturePool.Pull(secondPromise);
				IFuture futureAll = FuturePool.Pull(new[] {firstFuture, secondFuture});
				FuturePool.Release(futureAll);

				for (int i = 0; i < 2; i++)
				{
					Promise<uint> reusedPromise = PromisePool.Pull<uint>();
					IFuture<uint> reusedFuture = FuturePool.Pull(reusedPromise);
					Assert.IsTrue(reusedFuture.IsPending);
					Assert.IsTrue(reusedFuture == firstFuture || reusedFuture == secondFuture);
				}
			}

			[Test]
			public void ResolvedResultFutureAllReuse_AreSame()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> firstFuture = FuturePool.Pull(firstPromise);
				IFuture<uint> secondFuture = FuturePool.Pull(secondPromise);
				IFuture futureAll = FuturePool.Pull(new[] {firstFuture, secondFuture});
				firstPromise.Resolve(Result);
				secondPromise.Resolve(Result);
				FuturePool.Release(futureAll);

				for (int i = 0; i < 2; i++)
				{
					Promise<uint> reusedPromise = PromisePool.Pull<uint>();
					IFuture<uint> reusedFuture = FuturePool.Pull(reusedPromise);
					Assert.IsTrue(reusedFuture.IsPending);
					Assert.IsTrue(reusedFuture == firstFuture || reusedFuture == secondFuture);
				}
			}
		}

		private class Handler : IFutureHandler
		{
			public void HandleFuture(IFuture future)
			{
			}
		}

		[TestFixture]
		private class ReleaseTests
		{
			[Test]
			public void CheckStatusReleasedFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				FuturePool.Release(future);

				Assert.Throws<ForbiddenOperationException>(() => { bool _ = future.IsPending; });
				Assert.Throws<ForbiddenOperationException>(() => { bool _ = future.IsResolved; });
				Assert.Throws<ForbiddenOperationException>(() => { bool _ = future.IsRejected; });
			}

			[Test]
			public void CheckStatusReleasedResultFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				FuturePool.Release(future);

				Assert.Throws<ForbiddenOperationException>(() => { bool _ = future.IsPending; });
				Assert.Throws<ForbiddenOperationException>(() => { bool _ = future.IsResolved; });
				Assert.Throws<ForbiddenOperationException>(() => { bool _ = future.IsRejected; });
			}

			[Test]
			public void ThenReleasedFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				FuturePool.Release(future);

				Assert.Throws<ForbiddenOperationException>(() => future.Then(() => {}, reason => {}));
			}

			[Test]
			public void ThenReleasedResultFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				FuturePool.Release(future);

				Assert.Throws<ForbiddenOperationException>(() => future.Then(() => {}, reason => {}));
			}

			[Test]
			public void FinallyReleasedFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				FuturePool.Release(future);

				Handler handler = new Handler();
				Assert.Throws<ForbiddenOperationException>(() => future.Finally(handler));
			}

			[Test]
			public void FinallyReleasedResultFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				FuturePool.Release(future);

				Handler handler = new Handler();
				Assert.Throws<ForbiddenOperationException>(() => future.Finally(handler));
			}

			[Test]
			public void ChainFutureToReleasedFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(firstPromise);
				FuturePool.Release(future);

				Promise secondPromise = PromisePool.Pull();
				Assert.Throws<ForbiddenOperationException>(() => future.Then(() => FuturePool.Pull(secondPromise)));
			}

			[Test]
			public void ChainFutureToReleasedResultFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(firstPromise);
				FuturePool.Release(future);

				Promise secondPromise = PromisePool.Pull();
				Assert.Throws<ForbiddenOperationException>(() => future.Then(() => FuturePool.Pull(secondPromise)));
			}

			[Test]
			public void ChainResultFutureToReleasedFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(firstPromise);
				FuturePool.Release(future);

				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				Assert.Throws<ForbiddenOperationException>(() => future.Then(() => FuturePool.Pull(secondPromise)));
			}

			[Test]
			public void ChainResultFutureToReleasedResultFuture_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(firstPromise);
				FuturePool.Release(future);

				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				Assert.Throws<ForbiddenOperationException>(() => future.Then(() => FuturePool.Pull(secondPromise)));
			}
		}

		[TestFixture]
		private class ClearTests
		{
			[Test]
			public void ClearedFutureReuse_AreNotSame()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				FuturePool.Release(future);
				FuturePool.Clear();

				Promise reusedPromise = PromisePool.Pull();
				IFuture reusedFuture = FuturePool.Pull(reusedPromise);
				Assert.IsTrue(reusedFuture.IsPending);
				Assert.AreNotSame(future, reusedFuture);
			}

			[Test]
			public void ClearedResultFutureReuse_AreNotSame()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				FuturePool.Release(future);
				FuturePool.Clear();

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				IFuture<uint> reusedFuture = FuturePool.Pull(reusedPromise);
				Assert.IsTrue(reusedFuture.IsPending);
				Assert.AreNotSame(future, reusedFuture);
			}
		}
	}
}