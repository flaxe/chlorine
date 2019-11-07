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

				Assert.Throws<ChlorineException>(() =>
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

				Assert.Throws<ChlorineException>(() =>
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

				Assert.Throws<ChlorineException>(() =>
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

				Assert.Throws<ChlorineException>(() =>
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

				Assert.Throws<ChlorineException>(() =>
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

				Assert.Throws<ChlorineException>(() =>
				{
					Error _ = future.Reason;
				});
			}
		}

		private class Handler
		{
			public bool IsResolved { get; private set; }
			public bool IsRejected { get; private set; }

			public Error Error { get; private set; }

			public void OnResolve()
			{
				IsResolved = true;
			}

			public void OnReject(Error error)
			{
				IsRejected = true;
				Error = error;
			}
		}

		private class Handler<T>
		{
			public bool IsResolved { get; private set; }
			public bool IsRejected { get; private set; }

			public T Value { get; private set; }
			public Error Error { get; private set; }

			public void OnResolve(T value)
			{
				IsResolved = true;
				Value = value;
			}

			public void OnReject(Error error)
			{
				IsRejected = true;
				Error = error;
			}
		}

		[TestFixture]
		private class ThenTests
		{
			[Test]
			public void ThenBeforeFutureResolve_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				future.Then(handler.OnResolve, handler.OnReject);
				promise.Resolve();

				Assert.IsTrue(handler.IsResolved);
			}

			[Test]
			public void ThenBeforeResultFutureResolve_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				future.Then(handler.OnResolve, handler.OnReject);
				promise.Resolve(Result);

				Assert.IsTrue(handler.IsResolved);
				Assert.AreEqual(Result, handler.Value);
			}

			[Test]
			public void ThenAfterFutureResolve_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Resolve();
				future.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsResolved);
			}

			[Test]
			public void ThenAfterResultFutureResolve_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Resolve(Result);
				future.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsResolved);
				Assert.AreEqual(Result, handler.Value);
			}

			[Test]
			public void ThenBeforeFutureReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				future.Then(handler.OnResolve, handler.OnReject);
				promise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ThenBeforeResultFutureReject_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				future.Then(handler.OnResolve, handler.OnReject);
				promise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ThenAfterFutureReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Reject(Reason);
				future.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ThenAfterResultFutureReject_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Reject(Reason);
				future.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}
		}

		[TestFixture]
		private class CatchTests
		{
			[Test]
			public void CatchBeforeFutureReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				future.Catch(handler.OnReject);
				promise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void CatchBeforeResultFutureReject_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				future.Catch(handler.OnReject);
				promise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void CatchAfterFutureReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				promise.Reject(Reason);
				future.Catch(handler.OnReject);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void CatchAfterResultFutureReject_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				promise.Reject(Reason);
				future.Catch(handler.OnReject);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}
		}

		[TestFixture]
		private class ChainTests
		{
			[Test]
			public void ChainFutureToFutureResolve_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture chain = FuturePool.Pull(firstPromise).Then(() => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Resolve();
				secondPromise.Resolve();

				Assert.IsTrue(handler.IsResolved);
			}

			[Test]
			public void ChainResultFutureToFutureResolve_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise secondPromise = PromisePool.Pull();
				IFuture chain = FuturePool.Pull(firstPromise).Then(result => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Resolve(Result);
				secondPromise.Resolve();

				Assert.IsTrue(handler.IsResolved);
			}

			[Test]
			public void ChainFutureToResultFutureResolve_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise firstPromise = PromisePool.Pull();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.Pull(firstPromise).Then(() => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Resolve();
				secondPromise.Resolve(Result);

				Assert.IsTrue(handler.IsResolved);
				Assert.AreEqual(Result, handler.Value);
			}

			[Test]
			public void ChainResultFutureToResultFutureResolve_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.Pull(firstPromise).Then(result => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Resolve(Result);
				secondPromise.Resolve(Result);

				Assert.IsTrue(handler.IsResolved);
				Assert.AreEqual(Result, handler.Value);
			}

			[Test]
			public void ChainFutureToFutureFirstReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture chain = FuturePool.Pull(firstPromise).Then(() => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ChainFutureToFutureSecondReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture chain = FuturePool.Pull(firstPromise).Then(() => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Resolve();
				secondPromise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ChainResultFutureToFutureFirstReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise secondPromise = PromisePool.Pull();
				IFuture chain = FuturePool.Pull(firstPromise).Then(result => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ChainResultFutureToFutureSecondReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise secondPromise = PromisePool.Pull();
				IFuture chain = FuturePool.Pull(firstPromise).Then(result => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Resolve(Result);
				secondPromise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ChainFutureToResultFutureFirstReject_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise firstPromise = PromisePool.Pull();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.Pull(firstPromise).Then(() => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ChainFutureToResultFutureSecondReject_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise firstPromise = PromisePool.Pull();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.Pull(firstPromise).Then(() => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Resolve();
				secondPromise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ChainResultFutureToResultFutureFirstReject_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.Pull(firstPromise).Then(result => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ChainResultFutureToResultFutureSecondReject_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.Pull(firstPromise).Then(result => FuturePool.Pull(secondPromise));
				chain.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Resolve(Result);
				secondPromise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ResolvedChainFutureToFutureRelease_Release()
			{
				SharedPool.Clear();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture chain = FuturePool.Pull(firstPromise).Then(() => FuturePool.Pull(secondPromise));
				firstPromise.Resolve();
				secondPromise.Resolve();
				FuturePool.Release(chain);

				for (int i = 0; i < 2; i++)
				{
					IFuture future = SharedPool.Pull(typeof(Future)) as IFuture;
					Assert.IsNotNull(future);
					Assert.IsTrue(future.IsPending);
				}
			}

			[Test]
			public void ResolvedChainResultFutureToResultFuture_Release()
			{
				SharedPool.Clear();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.Pull(firstPromise).Then(result => FuturePool.Pull(secondPromise));
				firstPromise.Resolve(Result);
				secondPromise.Resolve(Result);
				FuturePool.Release(chain);

				for (int i = 0; i < 2; i++)
				{
					IFuture<uint> future = SharedPool.Pull(typeof(Future<uint>)) as IFuture<uint>;
					Assert.IsNotNull(future);
					Assert.IsTrue(future.IsPending);
				}
			}
		}
	}
}