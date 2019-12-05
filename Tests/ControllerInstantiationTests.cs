using System;
using System.Collections.Generic;
using Chlorine.Controller;
using Chlorine.Controller.Execution;
using Chlorine.Factories;
using Chlorine.Pools;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class ControllerInstantiationTests
	{
		private static readonly Error Reason = new Error(-100, "Test");

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
		}

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

		private class Executor : IExecutor<Executable>
		{
			public Executable CurrentExecutable { get; private set; }

			public void Execute(Executable executable, IExecutionHandler handler)
			{
				if (CurrentExecutable != null)
				{
					throw new Exception("Current executable not completed.");
				}
				CurrentExecutable = executable;
				CurrentExecutable.Execute(handler);
			}

			public void CompleteCurrent()
			{
				if (CurrentExecutable != null)
				{
					CurrentExecutable.Complete();
					CurrentExecutable = null;
				}
			}

			public void FailCurrent(Error reason)
			{
				if (CurrentExecutable != null)
				{
					CurrentExecutable.Fail(reason);
					CurrentExecutable = null;
				}
			}
		}

		[TestFixture]
		private class TransientTests
		{
			private class TransientDelegate : Executable, IActionDelegate<Foo>
			{
				public bool Init(Foo action)
				{
					return true;
				}
			}

			private class TransientResultDelegate : TransientDelegate, IActionDelegate<Foo, Bar>
			{
				public bool TryGetResult(out Bar result)
				{
					result = new Bar();
					return true;
				}
			}

			private class TransientDelegateFactory : IFactory<TransientDelegate>
			{
				public TransientDelegate Create()
				{
					return new TransientDelegate();
				}
			}

			private class TransientResultDelegateFactory : IFactory<TransientResultDelegate>
			{
				public TransientResultDelegate Create()
				{
					return new TransientResultDelegate();
				}
			}

			[Test]
			public void RepeatSucceedTransientAction_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().To<TransientDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedFromFactoryTransientAction_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().FromFactory<TransientDelegateFactory>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedTransientActionWithResult_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().To<TransientResultDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform<Foo, Bar>(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedFromFactoryTransientActionWithResult_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().FromFactory<TransientResultDelegateFactory>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform<Foo, Bar>(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatFailedTransientAction_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().To<TransientDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.FailCurrent(Reason);

				controller.Perform(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatFailedTransientActionWithResult_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().To<TransientResultDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.FailCurrent(Reason);

				controller.Perform<Foo, Bar>(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedTransientActionWithPool_AreSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().To<TransientDelegate>().AsReusable();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedTransientActionWithResultWithPool_AreSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().To<TransientResultDelegate>().AsReusable();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform<Foo, Bar>(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatFailedTransientActionWithPool_AreSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().To<TransientDelegate>().AsReusable();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.FailCurrent(Reason);

				controller.Perform(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatFailedTransientActionWithResultWithPool_AreSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().To<TransientResultDelegate>().AsReusable();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo());
				Executable firstExecutable = executor.CurrentExecutable;
				executor.FailCurrent(Reason);

				controller.Perform<Foo, Bar>(new Foo());
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreSame(firstExecutable, secondExecutable);
			}
		}

		[TestFixture]
		private class StackableTests
		{
			private class StackableDelegate : Executable, IStackableActionDelegate<Foo>
			{
				public readonly List<int> Codes = new List<int>();

				public bool Init(Foo action)
				{
					Codes.Add(action.Code);
					return true;
				}

				public bool Stack(Foo action)
				{
					Codes.Add(action.Code);
					return true;
				}
			}

			private class StackableResultDelegate : StackableDelegate, IStackableActionDelegate<Foo, Bar>
			{
				public bool TryGetResult(out Bar result)
				{
					result = new Bar();
					return true;
				}
			}

			private class StackableDelegateFactory : IFactory<StackableDelegate>
			{
				public StackableDelegate Create()
				{
					return new StackableDelegate();
				}
			}

			private class StackableResultDelegateFactory : IFactory<StackableResultDelegate>
			{
				public StackableResultDelegate Create()
				{
					return new StackableResultDelegate();
				}
			}

			[Test]
			public void RepeatPendingStackableAction_Stack()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().ToStackable<StackableDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo(10));
				StackableDelegate firstExecutable = executor.CurrentExecutable as StackableDelegate;

				controller.Perform(new Foo(20));
				StackableDelegate secondExecutable = executor.CurrentExecutable as StackableDelegate;

				Assert.AreSame(firstExecutable, secondExecutable);
				Assert.AreEqual(10, firstExecutable.Codes[0]);
				Assert.AreEqual(20, firstExecutable.Codes[1]);
			}

			[Test]
			public void RepeatPendingStackableActionWithResult_Stack()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().ToStackable<StackableResultDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo(10));
				StackableDelegate firstExecutable = executor.CurrentExecutable as StackableDelegate;

				controller.Perform<Foo, Bar>(new Foo(20));
				StackableDelegate secondExecutable = executor.CurrentExecutable as StackableDelegate;

				Assert.AreSame(firstExecutable, secondExecutable);
				Assert.AreEqual(10, firstExecutable.Codes[0]);
				Assert.AreEqual(20, firstExecutable.Codes[1]);
			}

			[Test]
			public void RepeatSucceedStackableAction_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().ToStackable<StackableDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedFromFactoryStackableAction_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().FromStackableFactory<StackableDelegateFactory>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedStackableActionWithResult_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().ToStackable<StackableResultDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform<Foo, Bar>(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedFromFactoryStackableActionWithResult_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().FromStackableFactory<StackableResultDelegateFactory>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform<Foo, Bar>(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatFailedStackableAction_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().ToStackable<StackableDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.FailCurrent(Reason);

				controller.Perform(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatFailedStackableActionWithResult_AreNotSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().ToStackable<StackableResultDelegate>().AsTransient();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.FailCurrent(Reason);

				controller.Perform<Foo, Bar>(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreNotSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedReusableStackableAction_AreSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().ToStackable<StackableDelegate>().AsReusable();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatSucceedReusableStackableActionWithResult_AreSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().ToStackable<StackableResultDelegate>().AsReusable();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.CompleteCurrent();

				controller.Perform<Foo, Bar>(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatFailedReusableStackableAction_AreSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().ToStackable<StackableDelegate>().AsReusable();

				IController controller = container.Resolve<IController>();

				controller.Perform(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.FailCurrent(Reason);

				controller.Perform(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreSame(firstExecutable, secondExecutable);
			}

			[Test]
			public void RepeatFailedReusableStackableActionWithResult_AreSame()
			{
				SharedPool.Clear();
				Container container = new Container();
				container.Extend<ControllerExtension>();
				Executor executor = new Executor();
				container.BindExecutable<Executable>().ToInstance(executor);
				container.BindAction<Foo>().With<Bar>().ToStackable<StackableResultDelegate>().AsReusable();

				IController controller = container.Resolve<IController>();

				controller.Perform<Foo, Bar>(new Foo(10));
				Executable firstExecutable = executor.CurrentExecutable;
				executor.FailCurrent(Reason);

				controller.Perform<Foo, Bar>(new Foo(20));
				Executable secondExecutable = executor.CurrentExecutable;

				Assert.AreSame(firstExecutable, secondExecutable);
			}
		}
	}
}