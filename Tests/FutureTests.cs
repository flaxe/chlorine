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
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Resolve();

				Assert.IsTrue(future.IsResolved);
			}

			[Test]
			public void ResultFutureResolve_IsResolved()
			{
				SharedPool.Clear();
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
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.IsTrue(future.IsRejected);
			}

			[Test]
			public void ResultFutureReject_IsRejected()
			{
				SharedPool.Clear();
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
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Resolve(Result);

				Assert.AreEqual(Result, future.Result);
			}

			[Test]
			public void ResolvedResultFutureTryGetResult_Equal()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Resolve(Result);

				Assert.IsTrue(future.TryGetResult(out uint result));
				Assert.AreEqual(Result, result);
			}

			[Test]
			public void PendingResultFutureGetResult_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					uint _ = future.Result;
				});
			}

			[Test]
			public void PendingResultFutureTryGetResult_False()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);

				Assert.IsFalse(future.TryGetResult(out uint _));
			}

			[Test]
			public void RejectedResultFutureGetResult_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					uint _ = future.Result;
				});
			}

			[Test]
			public void RejectedResultFutureTryGetResult_False()
			{
				SharedPool.Clear();
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
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.AreEqual(Reason, future.Reason);
			}

			[Test]
			public void RejectedResultFutureGetReason_Equal()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Reject(Reason);

				Assert.AreEqual(Reason, future.Reason);
			}

			[Test]
			public void PendingFutureGetReason_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					Error _ = future.Reason;
				});
			}

			[Test]
			public void PendingResultFutureGetReason_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					Error _ = future.Reason;
				});
			}

			[Test]
			public void ResolvedFutureGetReason_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Resolve();

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					Error _ = future.Reason;
				});
			}

			[Test]
			public void ResolvedResultFutureGetReason_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Resolve(Result);

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					Error _ = future.Reason;
				});
			}
		}

		[TestFixture]
		private class FutureAllTests
		{
			[Test]
			public void FutureAllResolve_IsResolved()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture futureAll = FuturePool.Pull(new []
				{
					FuturePool.Pull(firstPromise),
					FuturePool.Pull(secondPromise)
				});

				firstPromise.Resolve();
				Assert.IsTrue(futureAll.IsPending);

				secondPromise.Resolve();
				Assert.IsTrue(futureAll.IsResolved);
			}

			[Test]
			public void ResultFutureAllResolve_IsResolved()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture futureAll = FuturePool.Pull(new []
				{
					FuturePool.Pull(firstPromise),
					FuturePool.Pull(secondPromise)
				});

				firstPromise.Resolve(Result);
				Assert.IsTrue(futureAll.IsPending);

				secondPromise.Resolve(Result);
				Assert.IsTrue(futureAll.IsResolved);
			}

			[Test]
			public void FutureAllReject_IsRejected()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture futureAll = FuturePool.Pull(new []
				{
					FuturePool.Pull(firstPromise),
					FuturePool.Pull(secondPromise)
				});

				firstPromise.Reject(Reason);
				Assert.IsTrue(futureAll.IsRejected);

				secondPromise.Resolve();
				Assert.IsTrue(futureAll.IsRejected);
				Assert.AreEqual(Reason, futureAll.Reason);
			}

			[Test]
			public void ResultFutureAllReject_IsRejected()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture futureAll = FuturePool.Pull(new []
				{
					FuturePool.Pull(firstPromise),
					FuturePool.Pull(secondPromise)
				});

				firstPromise.Reject(Reason);
				Assert.IsTrue(futureAll.IsRejected);

				secondPromise.Resolve(Result);
				Assert.IsTrue(futureAll.IsRejected);
				Assert.AreEqual(Reason, futureAll.Reason);
			}
		}
	}
}