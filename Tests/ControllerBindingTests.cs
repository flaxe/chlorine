using Chlorine.Controller;
using Chlorine.Controller.Exceptions;
using Chlorine.Controller.Execution;
using Chlorine.Factories;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class ControllerBindingTests
	{
		private struct Foo
		{
		}

		private struct Bar
		{
		}

		private class FooCommand : IActionDelegate<Foo, Bar>
		{
			public bool IsPending => true;
			public bool IsSucceed => false;
			public bool IsFailed => false;

			public Error Error => default;

			public bool Init(Foo action)
			{
				return true;
			}

			public void Execute(IExecutionHandler handler)
			{
			}

			public bool TryGetResult(out Bar result)
			{
				result = new Bar();
				return true;
			}
		}

		private class FooExecutor : IExecutor<FooCommand>
		{
			public void Execute(FooCommand foo, IExecutionHandler handler)
			{
			}
		}

		[TestFixture]
		private class DuplicateTests
		{
			[Test]
			public void DuplicateActionBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindAction<Foo>().With<Bar>().To<FooCommand>().AsTransient();

				Assert.Throws<ControllerException>(() => container.BindAction<Foo>().To<FooCommand>().AsTransient());
			}

			[Test]
			public void DuplicateExecutableBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<FooCommand>().To<FooExecutor>().AsTransient();

				Assert.Throws<ControllerException>(() => container.BindExecutable<FooCommand>().To<FooExecutor>().AsTransient());
			}
		}

		private class FooExecutorFactory : IFactory<IExecutor<FooCommand>>
		{
			public IExecutor<FooCommand> Create()
			{
				return new FooExecutor();
			}
		}

		private class BarCommand : IActionDelegate<Bar>
		{
			public bool IsPending => true;
			public bool IsSucceed => false;
			public bool IsFailed => false;

			public Error Error => default;

			public bool Init(Bar action)
			{
				return true;
			}

			public void Execute(IExecutionHandler handler)
			{
			}
		}

		private class BarExecutor : IExecutor<BarCommand>
		{
			public void Execute(BarCommand bar, IExecutionHandler handler)
			{
			}
		}

		private class FooDelegateFactory : IFactory<IActionDelegate<Foo>>
		{
			public IActionDelegate<Foo> Create()
			{
				return new FooCommand();
			}
		}

		[TestFixture]
		private class IncompleteTests
		{
			[Test]
			public void IncompleteActionBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindAction<Foo>();

				Assert.Throws<ControllerException>(() => container.BindAction<Bar>().To<BarCommand>().AsTransient());
			}

			[Test]
			public void IncompleteActionWithResultBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindAction<Foo>().With<Bar>();

				Assert.Throws<ControllerException>(() => container.BindAction<Bar>().To<BarCommand>().AsTransient());
			}

			[Test]
			public void IncompleteCommandBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindAction<Foo>().To<FooCommand>();

				Assert.Throws<ControllerException>(() => container.BindAction<Bar>().To<BarCommand>().AsTransient());
			}

			[Test]
			public void IncompleteCommandFromFactoryBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindAction<Foo>().FromFactory<FooDelegateFactory>();

				Assert.Throws<ControllerException>(() => container.BindAction<Bar>().To<BarCommand>().AsTransient());
			}

			[Test]
			public void IncompleteActionPerform_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindAction<Foo>();

				IController controller = container.Resolve<IController>();
				Assert.Throws<ControllerException>(() => controller.Perform(new Foo()));
			}

			[Test]
			public void IncompleteExecutableBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<FooCommand>();

				Assert.Throws<ControllerException>(() => container.BindExecutable<BarCommand>().To<BarExecutor>().AsTransient());
			}

			[Test]
			public void IncompleteExecutorBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<FooCommand>().To<FooExecutor>();

				Assert.Throws<ControllerException>(() => container.BindExecutable<BarCommand>().To<BarExecutor>().AsTransient());
			}

			[Test]
			public void IncompleteExecutorFromFactoryBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindExecutable<FooCommand>().FromFactory<FooExecutorFactory>();

				Assert.Throws<ControllerException>(() => container.BindExecutable<BarCommand>().To<BarExecutor>().AsTransient());
			}

			[Test]
			public void IncompleteExecutablePerform_ExceptionThrown()
			{
				Container container = new Container();
				container.Extend<ControllerExtension>();
				container.BindAction<Foo>().To<FooCommand>().AsTransient();
				container.BindExecutable<FooCommand>();

				IController controller = container.Resolve<IController>();
				Assert.Throws<ControllerException>(() => controller.Perform(new Foo()));
			}
		}
	}
}