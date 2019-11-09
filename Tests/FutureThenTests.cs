using Chlorine.Futures;
using Chlorine.Pools;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class FutureThenTests
	{
		private static readonly uint Result = 5;
		private static readonly Error Reason = new Error(-100, "Test");

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
	}
}