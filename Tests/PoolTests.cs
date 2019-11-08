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
		private class PromiseTests
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
		}

		[TestFixture]
		private class FutureTests
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
		}
		
		[TestFixture]
		private class FutureChainTests
		{
			[Test]
			public void ReleasePendingChainFutureToFuture_Release()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture firstFuture = FuturePool.Pull(firstPromise);
				IFuture secondFuture = null;
				IFuture chainFuture = firstFuture.Then(() => secondFuture = FuturePool.Pull(secondPromise));
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
			public void ReleaseResolvedChainFutureToFuture_Release()
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
			public void ReleasePendingChainResultFutureToResultFuture_Release()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> firstFuture = FuturePool.Pull(firstPromise);
				IFuture<uint> secondFuture = null;
				IFuture<uint> chainFuture = firstFuture.Then(result => secondFuture = FuturePool.Pull(secondPromise));
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
			public void ReleaseResolvedChainResultFutureToResultFuture_Release()
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
		}
		
		[TestFixture]
		private class FutureAllTests
		{
			[Test]
			public void ReleasePendingFutureAll_Release()
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
			public void ReleaseResolvedFutureAll_Release()
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
			public void ReleasePendingResultFutureAll_Release()
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
			public void ReleaseResolvedResultFutureAll_Release()
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

		[TestFixture]
		private class ClearTests
		{
			[Test]
			public void ClearPromisePool_Empty()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				PromisePool.Release(promise);
				PromisePool.Clear();

				Promise reusedPromise = PromisePool.Pull();
				Assert.AreNotSame(promise, reusedPromise);
			}

			[Test]
			public void ClearResultPromisePool_Empty()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				PromisePool.Release(promise);
				PromisePool.Clear();

				Promise<uint> reusedPromise = PromisePool.Pull<uint>();
				Assert.AreNotSame(promise, reusedPromise);
			}
			
			[Test]
			public void ClearFuturePool_Empty()
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
			public void ClearResultFuturePool_Empty()
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