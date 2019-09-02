using Chlorine.Exceptions;
using Chlorine.Futures;
using Chlorine.Pools;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class PromiseTests
	{
		private static readonly uint Result = 5;
		private static readonly Error Reason = new Error(-100, "Test");

		[TestFixture]
		private class ResolveTests
		{
			[Test]
			public void PromiseResolve_IsResolved()
			{
				PromisePool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Resolve();

				Assert.IsTrue(promise.IsResolved);
			}

			[Test]
			public void PromiseRepeatedResolve_ExceptionThrown()
			{
				PromisePool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Resolve();

				Assert.Throws<ChlorineException>(() => promise.Resolve());
			}

			[Test]
			public void RejectedPromiseResolve_ExceptionThrown()
			{
				PromisePool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Reject(Reason);

				Assert.Throws<ChlorineException>(() => promise.Resolve());
			}

			[Test]
			public void ResultPromiseResolve_IsResolved()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.IsTrue(promise.IsResolved);
			}

			[Test]
			public void ResultPromiseRepeatedResolve_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.Throws<ChlorineException>(() => promise.Resolve(Result));
			}

			[Test]
			public void RejectedResultPromiseResolve_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.Throws<ChlorineException>(() => promise.Resolve(Result));
			}
		}

		[TestFixture]
		private class RejectTests
		{
			[Test]
			public void PromiseReject_IsRejected()
			{
				PromisePool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Reject(Reason);

				Assert.IsTrue(promise.IsRejected);
			}

			[Test]
			public void PromiseRepeatedReject_ExceptionThrown()
			{
				PromisePool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Reject(Reason);

				Assert.Throws<ChlorineException>(() => promise.Reject(Reason));
			}

			[Test]
			public void ResolvedPromiseReject_ExceptionThrown()
			{
				PromisePool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Resolve();

				Assert.Throws<ChlorineException>(() => promise.Reject(Reason));
			}

			[Test]
			public void ResultPromiseReject_IsRejected()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.IsTrue(promise.IsRejected);
			}

			[Test]
			public void ResultPromiseRepeatedReject_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.Throws<ChlorineException>(() => promise.Reject(Reason));
			}

			[Test]
			public void ResolvedResultPromiseReject_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.Throws<ChlorineException>(() => promise.Reject(Reason));
			}
		}

		[TestFixture]
		private class GetResultTests
		{
			[Test]
			public void ResolvedResultPromiseGetResult_Equal()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.AreEqual(Result, promise.Result);
			}

			[Test]
			public void ResolvedResultPromiseTryGetResult_Equal()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.IsTrue(promise.TryGetResult(out uint result));
				Assert.AreEqual(Result, result);
			}

			[Test]
			public void PendingResultPromiseGetResult_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();

				Assert.Throws<ChlorineException>(() =>
				{
					uint _ = promise.Result;
				});
			}

			[Test]
			public void PendingResultPromiseTryGetResult_False()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();

				Assert.IsFalse(promise.TryGetResult(out uint _));
			}

			[Test]
			public void RejectedResultPromiseGetResult_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.Throws<ChlorineException>(() =>
				{
					uint _ = promise.Result;
				});
			}

			[Test]
			public void RejectedResultPromiseTryGetResult_False()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.IsFalse(promise.TryGetResult(out uint _));
			}
		}

		[TestFixture]
		private class GetReasonTests
		{
			[Test]
			public void RejectedPromiseGetReason_Equal()
			{
				PromisePool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Reject(Reason);

				Assert.AreEqual(Reason, promise.Reason);
			}

			[Test]
			public void RejectedResultPromiseGetReason_Equal()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.AreEqual(Reason, promise.Reason);
			}

			[Test]
			public void PendingPromiseGetReason_ExceptionThrown()
			{
				PromisePool.Clear();
				Promise promise = PromisePool.Pull();

				Assert.Throws<ChlorineException>(() =>
				{
					Error _ = promise.Reason;
				});
			}

			[Test]
			public void PendingResultPromiseGetReason_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();

				Assert.Throws<ChlorineException>(() =>
				{
					Error _ = promise.Reason;
				});
			}

			[Test]
			public void ResolvedPromiseGetReason_ExceptionThrown()
			{
				PromisePool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Resolve();

				Assert.Throws<ChlorineException>(() =>
				{
					Error _ = promise.Reason;
				});
			}

			[Test]
			public void ResolvedResultPromiseGetReason_ExceptionThrown()
			{
				PromisePool.Clear<uint>();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.Throws<ChlorineException>(() =>
				{
					Error _ = promise.Reason;
				});
			}
		}

		[TestFixture]
		private class PoolTests
		{
			[Test]
			public void ReleaseResolvedPromise_Release()
			{
				PromisePool.Clear();
				Promise firstPromise = PromisePool.Pull();
				firstPromise.Resolve();
				PromisePool.Release(firstPromise);

				Promise secondPromise = PromisePool.Pull();
				Assert.IsTrue(secondPromise.IsPending);
				Assert.AreSame(firstPromise, secondPromise);
			}

			[Test]
			public void ReleaseResolvedResultPromise_Release()
			{
				PromisePool.Clear<uint>();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				firstPromise.Resolve(Result);
				PromisePool.Release(firstPromise);

				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				Assert.IsTrue(secondPromise.IsPending);
				Assert.AreSame(firstPromise, secondPromise);
			}

			[Test]
			public void ReleaseRejectedPromise_Release()
			{
				PromisePool.Clear();
				Promise firstPromise = PromisePool.Pull();
				firstPromise.Reject(Reason);
				PromisePool.Release(firstPromise);

				Promise secondPromise = PromisePool.Pull();
				Assert.IsTrue(secondPromise.IsPending);
				Assert.AreSame(firstPromise, secondPromise);
			}

			[Test]
			public void ReleaseRejectedResultPromise_Release()
			{
				PromisePool.Clear<uint>();
				Promise<uint> firstPromise = PromisePool.Pull<uint>();
				firstPromise.Reject(Reason);
				PromisePool.Release(firstPromise);

				Promise<uint> secondPromise = PromisePool.Pull<uint>();
				Assert.IsTrue(secondPromise.IsPending);
				Assert.AreSame(firstPromise, secondPromise);
			}
		}
	}
}