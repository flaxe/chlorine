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
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Resolve();

				Assert.IsTrue(promise.IsResolved);
			}

			[Test]
			public void PromiseRepeatedResolve_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Resolve();

				Assert.Throws<ForbiddenOperationException>(() => promise.Resolve());
			}

			[Test]
			public void RejectedPromiseResolve_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Reject(Reason);

				Assert.Throws<ForbiddenOperationException>(() => promise.Resolve());
			}

			[Test]
			public void ResultPromiseResolve_IsResolved()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.IsTrue(promise.IsResolved);
			}

			[Test]
			public void ResultPromiseRepeatedResolve_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.Throws<ForbiddenOperationException>(() => promise.Resolve(Result));
			}

			[Test]
			public void RejectedResultPromiseResolve_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.Throws<ForbiddenOperationException>(() => promise.Resolve(Result));
			}
		}

		[TestFixture]
		private class RejectTests
		{
			[Test]
			public void PromiseReject_IsRejected()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Reject(Reason);

				Assert.IsTrue(promise.IsRejected);
			}

			[Test]
			public void PromiseRepeatedReject_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Reject(Reason);

				Assert.Throws<ForbiddenOperationException>(() => promise.Reject(Reason));
			}

			[Test]
			public void ResolvedPromiseReject_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Resolve();

				Assert.Throws<ForbiddenOperationException>(() => promise.Reject(Reason));
			}

			[Test]
			public void ResultPromiseReject_IsRejected()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.IsTrue(promise.IsRejected);
			}

			[Test]
			public void ResultPromiseRepeatedReject_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.Throws<ForbiddenOperationException>(() => promise.Reject(Reason));
			}

			[Test]
			public void ResolvedResultPromiseReject_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.Throws<ForbiddenOperationException>(() => promise.Reject(Reason));
			}
		}

		[TestFixture]
		private class GetResultTests
		{
			[Test]
			public void ResolvedResultPromiseGetResult_Equal()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.AreEqual(Result, promise.Result);
			}

			[Test]
			public void ResolvedResultPromiseTryGetResult_Equal()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.IsTrue(promise.TryGetResult(out uint result));
				Assert.AreEqual(Result, result);
			}

			[Test]
			public void PendingResultPromiseGetResult_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					uint _ = promise.Result;
				});
			}

			[Test]
			public void PendingResultPromiseTryGetResult_False()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();

				Assert.IsFalse(promise.TryGetResult(out uint _));
			}

			[Test]
			public void RejectedResultPromiseGetResult_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					uint _ = promise.Result;
				});
			}

			[Test]
			public void RejectedResultPromiseTryGetResult_False()
			{
				SharedPool.Clear();
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
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Reject(Reason);

				Assert.AreEqual(Reason, promise.Reason);
			}

			[Test]
			public void RejectedResultPromiseGetReason_Equal()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Reject(Reason);

				Assert.AreEqual(Reason, promise.Reason);
			}

			[Test]
			public void PendingPromiseGetReason_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					Error _ = promise.Reason;
				});
			}

			[Test]
			public void PendingResultPromiseGetReason_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					Error _ = promise.Reason;
				});
			}

			[Test]
			public void ResolvedPromiseGetReason_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise promise = PromisePool.Pull();
				promise.Resolve();

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					Error _ = promise.Reason;
				});
			}

			[Test]
			public void ResolvedResultPromiseGetReason_ExceptionThrown()
			{
				SharedPool.Clear();
				Promise<uint> promise = PromisePool.Pull<uint>();
				promise.Resolve(Result);

				Assert.Throws<ForbiddenOperationException>(() =>
				{
					Error _ = promise.Reason;
				});
			}
		}
	}
}