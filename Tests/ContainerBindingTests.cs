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

		private class FooFactory : IFactory<IFoo>
		{
			public IFoo Create()
			{
				return new Foo();
			}
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

		[TestFixture]
		private class IncompleteTests
		{
			[Test]
			public void IncompleteBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>();

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>());
			}

			[Test]
			public void IncompleteWithIdBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A);

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>(A));
			}

			[Test]
			public void IncompleteTypeBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>().To<Foo>();

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>());
			}

			[Test]
			public void IncompleteFromFactoryBinding_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<IFoo>().FromFactory<FooFactory>();

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>());
			}
		}

		[TestFixture]
		private class RepeatedTests
		{
			private class Bar
			{
				public Foo Foo;

				[Inject]
				public void Init(Foo foo)
				{
					Foo = foo;
				}
			}

			private class Qux
			{
			}

			[Test]
			public void BindAfterResolve_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<Foo>().AsSingleton();
				container.Bind<Bar>().AsSingleton();

				Foo foo = container.Resolve<Foo>();

				Assert.NotNull(foo);
				Assert.Throws<ContainerException>(() => container.Bind<Qux>().AsSingleton());
			}

			[Test]
			public void BindAfterInstantiate_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<Foo>().AsSingleton();

				Bar bar = container.Instantiate<Bar>();

				Assert.NotNull(bar);
				Assert.Throws<ContainerException>(() => container.Bind<Qux>().AsSingleton());
			}

			[Test]
			public void BindAfterEmptyInstantiate_Bind()
			{
				Container container = new Container();

				Foo foo = container.Instantiate<Foo>();
				Assert.NotNull(foo);

				container.Bind<Qux>().AsSingleton();

				Qux qux = container.Resolve<Qux>();
				Assert.NotNull(qux);
			}

			[Test]
			public void BindAfterInject_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<Foo>().AsSingleton();

				Bar bar = new Bar();
				container.Inject(bar);

				Assert.Throws<ContainerException>(() => container.Bind<Qux>().AsSingleton());
			}

			[Test]
			public void BindAfterEmptyInject_Bind()
			{
				Container container = new Container();

				Foo foo = new Foo();
				container.Inject(foo);

				container.Bind<Qux>().AsSingleton();

				Qux qux = container.Resolve<Qux>();
				Assert.NotNull(qux);
			}
		}
	}
}