using System;
using Chlorine.Controller;
using Chlorine.Controller.Execution;
using Chlorine.Futures;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class ControllerPerformTests
	{
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

		private static readonly Foo Action = new Foo(43);

		private struct Bar
		{
			public readonly int Code;

			public Bar(int code)
			{
				Code = code;
			}
		}

		private static readonly Bar ActionResult = new Bar(10);

		private class Executable : IExecutable
		{
			private enum DelegateStatus
			{
				Pending,
				Succeed,
				Failed
			}

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
				_handler.HandleExecutable(this);
			}

			public void Fail(Error error)
			{
				_status = DelegateStatus.Failed;
				Error = error;
				_handler.HandleExecutable(this);
			}
		}

		private class ManualExecutor : IExecutor<Executable>
		{
			private Executable _currentExecutable;

			public void Execute(Executable executable, IExecutionHandler handler)
			{
				if (_currentExecutable != null)
				{
					throw new Exception("Current executable not completed.");
				}
				_currentExecutable = executable;
				_currentExecutable.Execute(handler);
			}

			public void CompleteCurrent()
			{
				if (_currentExecutable != null)
				{
					_currentExecutable.Complete();
					_currentExecutable = null;
				}
			}

			public void FailCurrent(Error reason)
			{
				if (_currentExecutable != null)
				{
					_currentExecutable.Fail(reason);
					_currentExecutable = null;
				}
			}
		}

		private class InstantExecutor : IExecutor<Executable>
		{
			public void Execute(Executable executable, IExecutionHandler handler)
			{
				executable.Execute(handler);
				executable.Complete();
			}
		}

		[TestFixture]
		private class InitializationTests
		{
			private class SucceedInitializationDelegate : Executable, IActionDelegate<Foo, Foo>
			{
				private Foo _action;

				public bool Init(Foo action)
				{
					_action = action;
					return true;
				}

				public Foo Result => _action;
			}

			[Test]
			public void ActionSucceedInitialization_AreEqual()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<Executable>().To<InstantExecutor>().AsSingleton();
				container.BindAction<Foo>().With<Foo>().To<SucceedInitializationDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Foo> future = controller.Perform<Foo, Foo>(Action);

				Assert.AreEqual(Action, future.Result);
			}

			private class FailedInitializationDelegate : Executable, IActionDelegate<Foo>
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
				container.BindExecutable<Executable>().To<InstantExecutor>().AsSingleton();
				container.BindAction<Foo>().To<FailedInitializationDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);

				Assert.IsTrue(future.IsRejected);
			}

			private class ExceptionalInitializationDelegate : Executable, IActionDelegate<Foo>
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
				container.BindExecutable<Executable>().To<InstantExecutor>().AsSingleton();
				container.BindAction<Foo>().To<ExceptionalInitializationDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);

				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Exception, future.Reason.ToException());
			}
		}

		[TestFixture]
		private class ExecutionTests
		{
			private class ActionDelegate : Executable, IActionDelegate<Foo, Bar>
			{
				public bool Init(Foo action)
				{
					return true;
				}

				public Bar Result => ActionResult;
			}

			[Test]
			public void ActionSucceedPerform_IsResolved()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				ManualExecutor executor = new ManualExecutor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().To<ActionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);
				Assert.IsTrue(future.IsPending);

				executor.CompleteCurrent();
				Assert.IsTrue(future.IsResolved);
			}

			[Test]
			public void ActionWithResultSucceedPerform_IsResolved()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				ManualExecutor executor = new ManualExecutor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().To<ActionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsPending);

				executor.CompleteCurrent();
				Assert.IsTrue(future.IsResolved);
				Assert.AreEqual(ActionResult, future.Result);
			}

			[Test]
			public void ActionSucceedInstantPerform_IsResolved()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<Executable>().To<InstantExecutor>().AsSingleton();
				container.BindAction<Foo>().To<ActionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);
				Assert.IsTrue(future.IsResolved);
			}

			[Test]
			public void ActionWithResultSucceedInstantPerform_IsResolved()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<Executable>().To<InstantExecutor>().AsSingleton();
				container.BindAction<Foo>().With<Bar>().To<ActionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsResolved);
				Assert.AreEqual(ActionResult, future.Result);
			}

			[Test]
			public void ActionFailedPerform_IsRejected()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				ManualExecutor executor = new ManualExecutor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().To<ActionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);
				Assert.IsTrue(future.IsPending);

				executor.FailCurrent(Reason);
				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Reason, future.Reason);
			}

			[Test]
			public void ActionWithResultFailedPerform_IsRejected()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				ManualExecutor executor = new ManualExecutor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().To<ActionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsPending);

				executor.FailCurrent(Reason);
				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Reason, future.Reason);
			}

			private class ExceptionalActionDelegate : Executable, IActionDelegate<Foo>
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
				container.BindExecutable<Executable>().To<InstantExecutor>().AsSingleton();
				container.BindAction<Foo>().To<ExceptionalActionDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture future = controller.Perform(Action);

				Assert.IsTrue(future.IsRejected);
				Assert.AreEqual(Exception, future.Reason.ToException());
			}
		}

		[TestFixture]
		private class GetResultTests
		{
			private class ResultDelegate : Executable, IActionDelegate<Foo, Bar>
			{
				public bool Init(Foo action)
				{
					return true;
				}

				public Bar Result => ActionResult;
			}

			[Test]
			public void ActionWithResultGetResult_AreEqual()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<Executable>().To<InstantExecutor>().AsSingleton();
				container.BindAction<Foo>().With<Bar>().To<ResultDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();
				IFuture<Bar> future = controller.Perform<Foo, Bar>(Action);
				Assert.IsTrue(future.IsResolved);
				Assert.AreEqual(ActionResult, future.Result);
			}
		}
	}
}