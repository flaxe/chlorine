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

		private class Handler
		{
			public bool IsResolved { get; private set; }
			public bool IsRejected { get; private set; }

			public bool IsPending => !IsResolved && !IsRejected;

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

			public bool IsPending => !IsResolved && !IsRejected;

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
		private class ThenFutureChainTests
		{
			[Test]
			public void ThenChainFutureToFutureResolve_Invoke()
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
			public void ThenChainResultFutureToFutureResolve_Invoke()
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
			public void ThenChainFutureToResultFutureResolve_Invoke()
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
			public void ThenChainResultFutureToResultFutureResolve_Invoke()
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
			public void ThenChainFutureToFutureFirstReject_Invoke()
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
			public void ThenChainFutureToFutureSecondReject_Invoke()
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
			public void ThenChainResultFutureToFutureFirstReject_Invoke()
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
			public void ThenChainResultFutureToFutureSecondReject_Invoke()
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
			public void ThenChainFutureToResultFutureFirstReject_Invoke()
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
			public void ThenChainFutureToResultFutureSecondReject_Invoke()
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
			public void ThenChainResultFutureToResultFutureFirstReject_Invoke()
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
			public void ThenChainResultFutureToResultFutureSecondReject_Invoke()
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
		}

		[TestFixture]
		private class ThenFutureAllTests
		{
			[Test]
			public void ThenBeforeFutureAllResolve_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture futureAll = FuturePool.Pull(new []
				{
					FuturePool.Pull(firstPromise),
					FuturePool.Pull(secondPromise)
				});
				futureAll.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Resolve();
				secondPromise.Resolve();

				Assert.IsTrue(handler.IsResolved);
			}

			[Test]
			public void ThenAfterFutureAllResolve_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture futureAll = FuturePool.Pull(new []
				{
					FuturePool.Pull(firstPromise),
					FuturePool.Pull(secondPromise)
				});
				firstPromise.Resolve();
				secondPromise.Resolve();
				futureAll.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsResolved);
			}

			[Test]
			public void ThenBeforeFutureAllReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture futureAll = FuturePool.Pull(new []
				{
					FuturePool.Pull(firstPromise),
					FuturePool.Pull(secondPromise)
				});
				futureAll.Then(handler.OnResolve, handler.OnReject);
				firstPromise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ThenAfterFutureAllReject_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise firstPromise = PromisePool.Pull();
				Promise secondPromise = PromisePool.Pull();
				IFuture futureAll = FuturePool.Pull(new []
				{
					FuturePool.Pull(firstPromise),
					FuturePool.Pull(secondPromise)
				});
				firstPromise.Reject(Reason);
				futureAll.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}
		}

		[TestFixture]
		private class PoolTests
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
			public void PendingChainFutureToFuture_Release()
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
			public void ResolvedChainFutureToFuture_Release()
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
			public void PendingChainResultFutureToResultFuture_Release()
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
			public void ResolvedChainResultFutureToResultFuture_Release()
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
			public void PendingFutureAll_Release()
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
			public void ResolvedFutureAll_Release()
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
			public void PendingResultFutureAll_Release()
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
			public void ResolvedResultFutureAll_Release()
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