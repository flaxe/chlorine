using Carbone.Exceptions;
using Carbone.Futures;
using Carbone.Pools;
using NUnit.Framework;

namespace Carbone.Tests
{
	internal class FutureThenTests
	{
		private static readonly uint Result = 5;
		private static readonly Error Reason = new Error(-100, "Test");

		private class Handler : IFutureHandler
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

			public void HandleFuture(IFuture future)
			{
				if (future.IsResolved)
				{
					OnResolve();
				}
				else if (future.IsRejected)
				{
					OnReject(future.Reason);
				}
			}
		}

		private class Handler<T> where T : new()
		{
			public bool IsResolved { get; private set; }
			public bool IsRejected { get; private set; }

			public T Value { get; private set; }
			public Error Error { get; private set; }

			public Handler()
			{
				Value = new T();
			}

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
			public void ThenChainFutureToResolvedFuture_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture chain = FuturePool.PullResolved().Then(() => FuturePool.Pull(promise));
				chain.Then(handler.OnResolve, handler.OnReject);
				promise.Resolve();

				Assert.IsTrue(handler.IsResolved);
			}

			[Test]
			public void ThenChainFutureToResultFutureResolve_Invoke()
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
			public void ThenChainFutureToResolvedResultFuture_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture chain = FuturePool.PullResolved(Result).Then(result => FuturePool.Pull(promise));
				chain.Then(handler.OnResolve, handler.OnReject);
				promise.Resolve();

				Assert.IsTrue(handler.IsResolved);
			}

			[Test]
			public void ThenChainResultFutureToFutureResolve_Invoke()
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
			public void ThenChainResultFutureToResolvedFuture_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.PullResolved().Then(() => FuturePool.Pull(promise));
				chain.Then(handler.OnResolve, handler.OnReject);
				promise.Resolve(Result);

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
			public void ThenChainResultFutureToResolvedResultFuture_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.PullResolved(Result).Then(result => FuturePool.Pull(promise));
				chain.Then(handler.OnResolve, handler.OnReject);
				promise.Resolve(Result);

				Assert.IsTrue(handler.IsResolved);
				Assert.AreEqual(Result, handler.Value);
			}

			[Test]
			public void ThenChainFutureToFutureReject_Invoke()
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
			public void ThenChainFutureRejectToFuture_Invoke()
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
			public void ThenChainFutureToRejectedFuture_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture chain = FuturePool.PullRejected(Reason).Then(() => FuturePool.Pull(promise));
				chain.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ThenChainFutureToResultFutureReject_Invoke()
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
			public void ThenChainFutureRejectToResultFuture_Invoke()
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
			public void ThenChainFutureToRejectedResultFuture_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture chain = FuturePool.PullRejected<uint>(Reason).Then(result => FuturePool.Pull(promise));
				chain.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ThenChainResultFutureToFutureReject_Invoke()
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
			public void ThenChainResultFutureRejectToFuture_Invoke()
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
			public void ThenChainResultFutureToRejectedFuture_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.PullRejected(Reason).Then(() => FuturePool.Pull(promise));
				chain.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ThenChainResultFutureToResultFutureReject_Invoke()
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
			public void ThenChainResultFutureRejectToResultFuture_Invoke()
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
			public void ThenChainResultFutureToRejectedResultFutureInvoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> chain = FuturePool.PullRejected<uint>(Reason).Then(result => FuturePool.Pull(promise));
				chain.Then(handler.OnResolve, handler.OnReject);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}
		}

		[TestFixture]
		private class ThenReleaseTests
		{
			[Test]
			public void ThenAfterResolvedFutureRelease_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				future.Then(() => FuturePool.Release(future), error => FuturePool.Release(future));
				future.Then(handler.OnResolve, handler.OnReject);
				promise.Resolve();

				Assert.IsTrue(handler.IsResolved);
			}

			[Test]
			public void ThenAfterRejectedFutureRelease_Invoke()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				future.Then(() => FuturePool.Release(future), error => FuturePool.Release(future));
				future.Then(handler.OnResolve, handler.OnReject);
				promise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void ThenAfterResolvedResultFutureRelease_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				future.Then(result => FuturePool.Release(future), error => FuturePool.Release(future));
				future.Then(handler.OnResolve, handler.OnReject);
				promise.Resolve(Result);

				Assert.IsTrue(handler.IsResolved);
				Assert.AreEqual(Result, handler.Value);
			}

			[Test]
			public void ThenAfterRejectedResultFutureRelease_Invoke()
			{
				SharedPool.Clear();
				Handler<uint> handler = new Handler<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				future.Then(result => FuturePool.Release(future), error => FuturePool.Release(future));
				future.Then(handler.OnResolve, handler.OnReject);
				promise.Reject(Reason);

				Assert.IsTrue(handler.IsRejected);
				Assert.AreEqual(Reason, handler.Error);
			}

			[Test]
			public void FinallyAfterResolvedFutureRelease_ExceptionThrown()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				future.Then(() => FuturePool.Release(future), error => FuturePool.Release(future));
				future.Finally(handler);

				Assert.Throws<ForbiddenOperationException>(promise.Resolve);
			}

			[Test]
			public void FinallyAfterRejectedFutureRelease_ExceptionThrown()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise promise = PromisePool.Pull();
				IFuture future = FuturePool.Pull(promise);
				future.Then(() => FuturePool.Release(future), error => FuturePool.Release(future));
				future.Finally(handler);

				Assert.Throws<ForbiddenOperationException>(() => promise.Reject(Reason));
			}

			[Test]
			public void FinallyAfterResolvedResultFutureRelease_ExceptionThrown()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				future.Then(() => FuturePool.Release(future), error => FuturePool.Release(future));
				future.Finally(handler);

				Assert.Throws<ForbiddenOperationException>(() => promise.Resolve(Result));
			}

			[Test]
			public void FinallyAfterRejectedResultFutureRelease_ExceptionThrown()
			{
				SharedPool.Clear();
				Handler handler = new Handler();
				Promise<uint> promise = PromisePool.Pull<uint>();
				IFuture<uint> future = FuturePool.Pull(promise);
				future.Then(() => FuturePool.Release(future), error => FuturePool.Release(future));
				future.Finally(handler);

				Assert.Throws<ForbiddenOperationException>(() => promise.Reject(Reason));
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
	}
}