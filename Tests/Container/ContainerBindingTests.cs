using Carbone.Exceptions;
using NUnit.Framework;

namespace Carbone.Tests
{
	internal class ContainerBindingTests
	{
		private const string A = "A";

		private interface IFoo
		{
		}

		private class Foo : IFoo
		{
		}

		private interface IBar
		{
		}

		private class Bar : IBar
		{
		}

		[TestFixture]
		private class DuplicateTests
		{
			[Test]
			public void DuplicateBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<Foo>().AsTransient();

				Assert.Throws<ContainerException>(() => container.Bind<Foo>().AsSingleton());
			}

			[Test]
			public void DuplicateWithIdBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<Foo>().WithId(A).AsTransient();

				Assert.Throws<ContainerException>(() => container.Bind<Foo>().WithId(A).AsTransient());
			}

			[Test]
			public void DuplicateMixedBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<Foo>().AsTransient();

				Assert.Throws<ContainerException>(() => container.Bind<Foo>().When(context => context.Optional).AsSingleton());
			}

			[Test]
			public void DuplicateConditionalBinding_ExceptionThrown()
			{
				BindingCondition condition = context => context.Optional;
				Container container = new Container();
				container.Bind<Foo>().When(condition).AsTransient();

				Assert.Throws<ContainerException>(() => container.Bind<Foo>().When(condition).AsTransient());
			}

			[Test]
			public void DuplicateDifferentConditionalBinding_DoesNotThrowException()
			{
				Container container = new Container();
				container.Bind<Foo>().When(context => !context.Optional).AsTransient();

				Assert.DoesNotThrow(() => container.Bind<Foo>().When(context => context.Optional).AsTransient());
			}

			[Test]
			public void DuplicateWhenInjectIntoBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<Foo>().WhenInjectInto<Bar>().AsTransient();

				Assert.Throws<ContainerException>(() => container.Bind<Foo>().WhenInjectInto<Bar>().AsTransient());
			}
		}

		private class FooFactory : IFactory<IFoo>
		{
			public IFoo Create()
			{
				return new Foo();
			}
		}

		private abstract class Qux
		{
		}

		private abstract class QuxFactory : IFactory<IFoo>
		{
			public IFoo Create()
			{
				return new Foo();
			}
		}

		[TestFixture]
		private class InvalidTests
		{
			[Test]
			public void InterfaceBinding_ExceptionThrown()
			{
				Container container = new Container();

				Assert.Throws<InvalidArgumentException>(() => container.Bind<IFoo>().AsTransient());
			}

			[Test]
			public void FactoryInterfaceBinding_ExceptionThrown()
			{
				Container container = new Container();

				Assert.Throws<InvalidArgumentException>(() => container.Bind<IFoo>().FromFactory<IFactory<IFoo>>().AsTransient());
			}

			[Test]
			public void AbstractBinding_ExceptionThrown()
			{
				Container container = new Container();

				Assert.Throws<InvalidArgumentException>(() => container.Bind<Qux>().AsTransient());
			}

			[Test]
			public void AbstractFactoryBinding_ExceptionThrown()
			{
				Container container = new Container();

				Assert.Throws<InvalidArgumentException>(() => container.Bind<IFoo>().FromFactory<QuxFactory>().AsTransient());
			}
		}

		[TestFixture]
		private class IncompleteTests
		{
			[Test]
			public void IncompleteBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>();

				Assert.Throws<ContainerException>(() => container.Bind<IBar>().To<Bar>().AsTransient());
			}

			[Test]
			public void IncompleteWithIdBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A);

				Assert.Throws<ContainerException>(() => container.Bind<IBar>().To<Bar>().AsTransient());
			}

			[Test]
			public void IncompleteConditionalBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>().When(context => context.Optional);

				Assert.Throws<ContainerException>(() => container.Bind<IBar>().To<Bar>().AsTransient());
			}

			[Test]
			public void IncompleteTypeBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>().To<Foo>();

				Assert.Throws<ContainerException>(() => container.Bind<IBar>().To<Bar>().AsTransient());
			}

			[Test]
			public void IncompleteFromFactoryBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>().FromFactory<FooFactory>();

				Assert.Throws<ContainerException>(() => container.Bind<IBar>().To<Bar>().AsTransient());
			}

			[Test]
			public void IncompleteResolve_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>();

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>());
			}
		}
	}
}