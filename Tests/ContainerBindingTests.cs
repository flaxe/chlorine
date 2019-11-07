using Chlorine.Exceptions;
using Chlorine.Factories;
using NUnit.Framework;

namespace Chlorine.Tests
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

		[TestFixture]
		private class DuplicateTests
		{
			[Test]
			public void DuplicateBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>().AsTransient();

				Assert.Throws<ContainerException>(() => container.Bind<IFoo>().AsSingleton());
			}

			[Test]
			public void DuplicateWithIdBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).AsTransient();

				Assert.Throws<ContainerException>(() => container.Bind<IFoo>().WithId(A).AsTransient());
			}
		}

		private interface IBar
		{
		}

		private class Bar : IBar
		{
		}

		private class FooFactory : IFactory<IFoo>
		{
			public IFoo Create()
			{
				return new Foo();
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