using System;
using Chlorine.Controller;
using Chlorine.Controller.Execution;
using Chlorine.Futures;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class ControllerPerformTests
	{
		private static readonly Foo Action = new Foo(43);
		private static readonly Bar Result = new Bar(10);

		private static readonly Error Reason = new Error(-100, "Test");
		private static readonly Exception Exception = new Exception("Test");

		private struct Foo
		{
			public readonly int Code;

			public Foo(int code)
			{
				Code = code;
			}
		}

		private struct Bar
		{
			public readonly int Code;

			public Bar(int code)
			{
				Code = code;
			}
		}

		private interface IExecutableDelegate : IExecutable
		{
		}

		private class Executor : IExecutor<IExecutableDelegate>
		{
			public void Execute(IExecutableDelegate command, IExecutionHandler handler)
			{
				command.Execute(handler);
			}
		}

		[TestFixture]
		private class InitializationTests
		{
			private class InitializationDelegate : IExecutableDelegate
			{
				public bool IsPending => true;
				public bool IsSucceed => false;
				public bool IsFailed => false;

				public Error Error => default;

				public void Execute(IExecutionHandler handler)
				{
				}
			}

			private class SucceedInitializationDelegate : InitializationDelegate, IActionDelegate<Foo>
			{
				public static SucceedInitializationDelegate Instance { get; private set; }

				public Foo Action { get; private set; }

				public SucceedInitializationDelegate()
				{
					Instance = this;
				}

				public bool Init(Foo action)
				{
					Action = action;
					return true;
				}
			}

			[Test]
			public void ActionSucceedInitialization_AreEqual()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().To<SucceedInitializationDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				controller.Perform(Action);

				Assert.AreEqual(Action, SucceedInitializationDelegate.Instance.Action);
			}

			private class FailedInitializationDelegate : InitializationDelegate, IActionDelegate<Foo>
			{
				public bool Init(Foo action)
				{
					return false;
				}
			}

			[Test]
			public void ActionFailedInitialization_IsRejected()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().To<FailedInitializationDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);

				Assert.IsTrue(future.IsRejected);
			}

			private class ExceptionalInitializationDelegate : InitializationDelegate, IActionDelegate<Foo>
			{
				public bool Init(Foo action)
				{
					throw Exception;
				}
			}

			[Test]
			public void ActionInitializationWithException_IsRejected()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().To<ExceptionalInitializationDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);

				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Exception, future.Reason.Exception.InnerException);
			}
		}

		[TestFixture]
		private class ExecutionTests
		{
			private enum DelegateStatus
			{
				Pending,
				Succeed,
				Failed
			}

			private class ExecutionDelegate : IExecutableDelegate
			{
				private IExecutionHandler _handler;
				private DelegateStatus _status = DelegateStatus.Pending;

				public bool IsPending => _status == DelegateStatus.Pending;
				public bool IsSucceed => _status == DelegateStatus.Succeed;
				public bool IsFailed => _status == DelegateStatus.Failed;

				public Error Error { get; private set; }

				public virtual void Execute(IExecutionHandler handler)
				{
					_handler = handler;
				}

				public void Complete()
				{
					_status = DelegateStatus.Succeed;
					_handler.HandleComplete(this);
				}

				public void Fail(Error error)
				{
					_status = DelegateStatus.Failed;
					Error = error;
					_handler.HandleComplete(this);
				}
			}

			private class ManualExecutionDelegate : ExecutionDelegate, IActionDelegate<Foo, Bar>
			{
				public static ManualExecutionDelegate Instance { get; private set; }

				public ManualExecutionDelegate()
				{
					Instance = this;
				}

				public bool Init(Foo action)
				{
					return true;
				}

				public bool TryGetResult(out Bar result)
				{
					result = Result;
					return true;
				}
			}

			[Test]
			public void ActionSucceedPerform_IsResolved()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().To<ManualExecutionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);
				Assert.IsTrue(future.IsPending);

				ManualExecutionDelegate.Instance.Complete();
				Assert.IsTrue(future.IsResolved);
			}

			[Test]
			public void ActionWithResultSucceedPerform_IsResolved()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().With<Bar>().To<ManualExecutionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsPending);

				ManualExecutionDelegate.Instance.Complete();
				Assert.IsTrue(future.IsResolved);
				Assert.AreEqual(Result, future.Result);
			}

			private class InstantExecutionDelegate : ExecutionDelegate, IActionDelegate<Foo, Bar>
			{
				public static InstantExecutionDelegate Instance { get; private set; }

				public InstantExecutionDelegate()
				{
					Instance = this;
				}

				public bool Init(Foo action)
				{
					return true;
				}

				public override void Execute(IExecutionHandler handler)
				{
					base.Execute(handler);
					Complete();
				}

				public bool TryGetResult(out Bar result)
				{
					result = Result;
					return true;
				}
			}

			[Test]
			public void ActionSucceedInstantPerform_IsResolved()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().To<InstantExecutionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);
				Assert.IsTrue(future.IsResolved);
			}

			[Test]
			public void ActionWithResultSucceedInstantPerform_IsResolved()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().With<Bar>().To<InstantExecutionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsResolved);
				Assert.AreEqual(Result, future.Result);
			}

			[Test]
			public void ActionFailedPerform_IsRejected()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().To<ManualExecutionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);
				Assert.IsTrue(future.IsPending);

				ManualExecutionDelegate.Instance.Fail(Reason);
				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Reason, future.Reason);
			}

			[Test]
			public void ActionWithResultFailedPerform_IsRejected()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().With<Bar>().To<ManualExecutionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsPending);

				ManualExecutionDelegate.Instance.Fail(Reason);
				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Reason, future.Reason);
			}

			private class ExceptionalExecutionDelegate : ExecutionDelegate, IActionDelegate<Foo>
			{
				public bool Init(Foo action)
				{
					return true;
				}

				public override void Execute(IExecutionHandler handler)
				{
					throw Exception;
				}
			}

			[Test]
			public void ActionPerformWithException_IsRejected()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().To<ExceptionalExecutionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);

				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Exception, future.Reason.Exception.InnerException);
			}
		}

		[TestFixture]
		private class GetResultTests
		{
			private class ResultDelegate : IExecutableDelegate
			{
				public bool IsPending => false;
				public bool IsSucceed => true;
				public bool IsFailed => false;

				public Error Error => default;

				public void Execute(IExecutionHandler handler)
				{
				}
			}

			private class SucceedResultDelegate : ResultDelegate, IActionDelegate<Foo, Bar>
			{
				public bool Init(Foo action)
				{
					return true;
				}

				public bool TryGetResult(out Bar result)
				{
					result = Result;
					return true;
				}
			}

			[Test]
			public void ActionWithResultSucceedGetResult_AreEqual()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().With<Bar>().To<SucceedResultDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsResolved);
				Assert.AreEqual(Result, future.Result);
			}

			private class FailedResultDelegate : ResultDelegate, IActionDelegate<Foo, Bar>
			{
				public bool Init(Foo action)
				{
					return true;
				}

				public bool TryGetResult(out Bar result)
				{
					result = default;
					return false;
				}
			}

			[Test]
			public void ActionWithResultFailedGetResult_IsRejected()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().With<Bar>().To<FailedResultDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsRejected);
			}

			private class ExceptionalResultDelegate : ResultDelegate, IActionDelegate<Foo, Bar>
			{
				public bool Init(Foo action)
				{
					return true;
				}

				public bool TryGetResult(out Bar result)
				{
					throw Exception;
				}
			}

			[Test]
			public void ActionWithResultGetResultWithException_IsRejected()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<IExecutableDelegate>().To<Executor>().AsSingleton();
				container.BindAction<Foo>().With<Bar>().To<ExceptionalResultDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsRejected);
			}
		}
	}
}