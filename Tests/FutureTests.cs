using Chlorine.Exceptions;
using Chlorine.Futures;
using Chlorine.Pools;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class FutureTests
	{
		private static readonly uint Result = 5;
		private static readonly Error Reason = new Error(-100, "Test");

		[TestFixture]
		private class ResolveTests
		{
			[Test]
			public void FutureResolve_IsResolved()
			{
				PromisePool.Clear();
				FuturePool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Resolve();

				Assert.IsTrue(future.IsResolved);
			}

			[Test]
			public void ResultFutureResolve_IsResolved()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Resolve(Result);

				Assert.IsTrue(future.IsResolved);
			}
		}

		[TestFixture]
		private class RejectTests
		{
			[Test]
			public void FutureReject_IsRejected()
			{
				PromisePool.Clear();
				FuturePool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.IsTrue(future.IsRejected);
			}

			[Test]
			public void ResultFutureReject_IsRejected()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.IsTrue(future.IsRejected);
			}
		}

		[TestFixture]
		private class GetResultTests
		{
			[Test]
			public void ResolvedResultFutureGetResult_Equal()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Resolve(Result);

				Assert.AreEqual(Result, future.Result);
			}

			[Test]
			public void ResolvedResultFutureTryGetResult_Equal()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Resolve(Result);

				Assert.IsTrue(future.TryGetResult(out uint result));
				Assert.AreEqual(Result, result);
			}

			[Test]
			public void PendingResultFutureGetResult_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);

				Assert.Throws<ChlorineException>(() =>
				{
					uint _ = future.Result;
				});
			}

			[Test]
			public void PendingResultFutureTryGetResult_False()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);

				Assert.IsFalse(future.TryGetResult(out uint _));
			}

			[Test]
			public void RejectedResultFutureGetResult_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.Throws<ChlorineException>(() =>
				{
					uint _ = future.Result;
				});
			}

			[Test]
			public void RejectedResultFutureTryGetResult_False()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.IsFalse(future.TryGetResult(out uint _));
			}
		}

		[TestFixture]
		private class GetReasonTests
		{
			[Test]
			public void RejectedFutureGetReason_Equal()
			{
				PromisePool.Clear();
				FuturePool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.AreEqual(Reason, future.Reason);
			}

			[Test]
			public void RejectedResultFutureGetReason_Equal()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.AreEqual(Reason, future.Reason);
			}

			[Test]
			public void PendingFutureGetReason_ExceptionThrown()
			{
				PromisePool.Clear();
				FuturePool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);

				Assert.Throws<ChlorineException>(() =>
				{
					Error _ = future.Reason;
				});
			}

			[Test]
			public void PendingResultFutureGetReason_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);

				Assert.Throws<ChlorineException>(() =>
				{
					Error _ = future.Reason;
				});
			}

			[Test]
			public void ResolvedFutureGetReason_ExceptionThrown()
			{
				PromisePool.Clear();
				FuturePool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Resolve();

				Assert.Throws<ChlorineException>(() =>
				{
					Error _ = future.Reason;
				});
			}

			[Test]
			public void ResolvedResultFutureGetReason_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Resolve(Result);

				Assert.Throws<ChlorineException>(() =>
				{
					Error _ = future.Reason;
				});
			}
		}

		[TestFixture]
		private class PoolTests
		{
			[Test]
			public void ReleaseResolvedFuture_Release()
			{
				PromisePool.Clear();
				FuturePool.Clear();

				Promise firstPromise = PromisePool.Pull();
				IFuture firstFuture = FuturePool.Pull(firstPromise);
				firstPromise.Resolve();
				FuturePool.Release(firstFuture);

				Promise secondPromise = PromisePool.Pull();
				IFuture secondFuture = FuturePool.Pull(secondPromise);
				Assert.IsTrue(secondFuture.IsPending);
				Assert.AreSame(firstFuture, secondFuture);
			}

			[Test]
			public void ReleaseResolvedResultFuture_Release()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();

				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				IFuture<uint> firstFuture = FuturePool.Pull(firstPromise);
				firstPromise.Resolve(Result);
				FuturePool.Release(firstFuture);

				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> secondFuture = FuturePool.Pull(secondPromise);
				Assert.IsTrue(secondFuture.IsPending);
				Assert.AreSame(firstFuture, secondFuture);
			}

			[Test]
			public void ReleaseRejectedFuture_Release()
			{
				PromisePool.Clear();
				FuturePool.Clear();

				Promise firstPromise = PromisePool.Pull();
				IFuture firstFuture = FuturePool.Pull(firstPromise);
				firstPromise.Reject(Reason);
				FuturePool.Release(firstFuture);

				Promise secondPromise = PromisePool.Pull();
				IFuture secondFuture = FuturePool.Pull(secondPromise);
				Assert.IsTrue(secondFuture.IsPending);
				Assert.AreSame(firstFuture, secondFuture);
			}

			[Test]
			public void ReleaseRejectedResultFuture_Release()
			{
				PromisePool.Clear<uint>();
				FuturePool.Clear<uint>();

				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				IFuture<uint> firstFuture = FuturePool.Pull(firstPromise);
				firstPromise.Reject(Reason);
				FuturePool.Release(firstFuture);

				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> secondFuture = FuturePool.Pull(secondPromise);
				Assert.IsTrue(secondFuture.IsPending);
				Assert.AreSame(firstFuture, secondFuture);
			}
		}
	}
}