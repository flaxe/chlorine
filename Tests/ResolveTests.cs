using Chlorine.Factories;
using NUnit.Framework;

namespace Chlorine.Tests
{
	[TestFixture]
	internal class ResolveInstanceTests
	{
		private static readonly object A = "a";
		private static readonly object B = "b";

		[Test]
		public void ResolveByType()
		{
			Foo instance = new Foo();

			Container container = new Container();
			container.Bind<Foo>().ToInstance(instance);

			Assert.IsNull(container.TryResolve<IFoo>());
			Assert.AreSame(instance, container.Resolve<Foo>());
		}

		[Test]
		public void ResolveByTypeWithId()
		{
			Foo instanceA = new Foo();
			Foo instanceB = new Foo();

			Container container = new Container();
			container.Bind<Foo>().WithId(A).ToInstance(instanceA);
			container.Bind<Foo>().WithId(B).ToInstance(instanceB);

			Assert.IsNull(container.TryResolve<Foo>());
			Assert.IsNull(container.TryResolve<IFoo>());
			Assert.IsNull(container.TryResolve<IFoo>(A));
			Assert.IsNull(container.TryResolve<IFoo>(B));

			Foo fooA = container.Resolve<Foo>(A);
			Foo fooB = container.Resolve<Foo>(B);

			Assert.AreSame(instanceA, fooA);
			Assert.AreSame(instanceB, fooB);
			Assert.AreNotSame(fooA, fooB);
		}

		[Test]
		public void ResolveByInterface()
		{
			IFoo instance = new Foo();

			Container container = new Container();
			container.Bind<IFoo>().ToInstance(instance);

			Assert.IsNull(container.TryResolve<Foo>());
			Assert.AreSame(instance, container.Resolve<IFoo>());
		}

		[Test]
		public void ResolveByInterfaceWithId()
		{
			IFoo instanceA = new Foo();
			IFoo instanceB = new Foo();

			Container container = new Container();
			container.Bind<IFoo>().WithId(A).ToInstance(instanceA);
			container.Bind<IFoo>().WithId(B).ToInstance(instanceB);

			Assert.IsNull(container.TryResolve<IFoo>());
			Assert.IsNull(container.TryResolve<Foo>());
			Assert.IsNull(container.TryResolve<Foo>(A));
			Assert.IsNull(container.TryResolve<Foo>(B));

			IFoo fooA = container.Resolve<IFoo>(A);
			IFoo fooB = container.Resolve<IFoo>(B);

			Assert.AreSame(instanceA, fooA);
			Assert.AreSame(instanceB, fooB);
			Assert.AreNotSame(fooA, fooB);
		}
	}

	[TestFixture]
	internal class ResolveSingletonTests
	{
		private static readonly object A = "a";
		private static readonly object B = "b";

		[Test]
		public void ResolveByType()
		{
			Container container = new Container();
			container.Bind<Foo>().AsSingleton();

			Assert.IsNull(container.TryResolve<IFoo>());

			Foo foo = container.Resolve<Foo>();
			Assert.NotNull(foo);
			Assert.AreSame(foo, container.Resolve<Foo>());
		}

		[Test]
		public void ResolveByTypeWithId()
		{
			Container container = new Container();
			container.Bind<Foo>().WithId(A).AsSingleton();
			container.Bind<Foo>().WithId(B).AsSingleton();

			Assert.IsNull(container.TryResolve<Foo>());
			Assert.IsNull(container.TryResolve<IFoo>());
			Assert.IsNull(container.TryResolve<IFoo>(A));
			Assert.IsNull(container.TryResolve<IFoo>(B));

			Foo fooA = container.Resolve<Foo>(A);
			Foo fooB = container.Resolve<Foo>(B);

			Assert.NotNull(fooA);
			Assert.NotNull(fooB);
			Assert.AreNotSame(fooA, fooB);
			Assert.AreSame(fooA, container.Resolve<Foo>(A));
			Assert.AreSame(fooB, container.Resolve<Foo>(B));
		}

		[Test]
		public void ResolveByInterface()
		{
			Container container = new Container();
			container.Bind<IFoo>().To<Foo>().AsSingleton();

			Test(container);
		}

		[Test]
		public void ResolveByInterfaceWithId()
		{
			Container container = new Container();
			container.Bind<IFoo>().WithId(A).To<Foo>().AsSingleton();
			container.Bind<IFoo>().WithId(B).To<Foo>().AsSingleton();

			TestWithId(container);
		}

		[Test]
		public void ResolveFromFactory()
		{
			Container container = new Container();
			container.Bind<IFoo>().FromFactory<FooFactory>().AsSingleton();

			Test(container);
		}

		[Test]
		public void ResolveFromFactoryWithId()
		{
			Container container = new Container();
			container.Bind<IFoo>().WithId(A).FromFactory<FooFactory>().AsSingleton();
			container.Bind<IFoo>().WithId(B).FromFactory<FooFactory>().AsSingleton();

			TestWithId(container);
		}

		[Test]
		public void ResolveFromFactoryMethod()
		{
			FooFactory factory = new FooFactory();

			Container container = new Container();
			container.Bind<IFoo>().FromFactoryMethod(factory.Create).AsSingleton();

			Test(container);
		}

		[Test]
		public void ResolveFromFactoryMethodWithId()
		{
			FooFactory factory = new FooFactory();

			Container container = new Container();
			container.Bind<IFoo>().WithId(A).FromFactoryMethod(factory.Create).AsSingleton();
			container.Bind<IFoo>().WithId(B).FromFactoryMethod(factory.Create).AsSingleton();

			TestWithId(container);
		}

		private void Test(Container container)
		{
			Assert.IsNull(container.TryResolve<Foo>());

			IFoo foo = container.Resolve<IFoo>();
			Assert.NotNull(foo);
			Assert.AreSame(foo, container.Resolve<IFoo>());
		}

		private void TestWithId(Container container)
		{
			Assert.IsNull(container.TryResolve<IFoo>());
			Assert.IsNull(container.TryResolve<Foo>());
			Assert.IsNull(container.TryResolve<Foo>(A));
			Assert.IsNull(container.TryResolve<Foo>(B));

			IFoo fooA = container.Resolve<IFoo>(A);
			IFoo fooB = container.Resolve<IFoo>(B);

			Assert.NotNull(fooA);
			Assert.NotNull(fooB);
			Assert.AreNotSame(fooA, fooB);
			Assert.AreSame(fooA, container.Resolve<IFoo>(A));
			Assert.AreSame(fooB, container.Resolve<IFoo>(B));
		}
	}

	[TestFixture]
	internal class ResolveTransientTests
	{
		private static readonly object A = "a";
		private static readonly object B = "b";

		[Test]
		public void ResolveByType()
		{
			Container container = new Container();
			container.Bind<Foo>().AsTransient();

			Assert.IsNull(container.TryResolve<IFoo>());

			Foo foo = container.Resolve<Foo>();
			Assert.NotNull(foo);

			Foo anotherFoo = container.Resolve<Foo>();
			Assert.NotNull(anotherFoo);
			Assert.AreNotSame(foo, anotherFoo);
		}

		[Test]
		public void ResolveByTypeWithId()
		{
			Container container = new Container();
			container.Bind<Foo>().WithId(A).AsTransient();
			container.Bind<Foo>().WithId(B).AsTransient();

			Assert.IsNull(container.TryResolve<Foo>());
			Assert.IsNull(container.TryResolve<IFoo>());
			Assert.IsNull(container.TryResolve<IFoo>(A));
			Assert.IsNull(container.TryResolve<IFoo>(B));

			Foo fooA = container.Resolve<Foo>(A);
			Foo fooB = container.Resolve<Foo>(B);
			Assert.NotNull(fooA);
			Assert.NotNull(fooB);
			Assert.AreNotSame(fooA, fooB);

			Foo anotherFooA = container.Resolve<Foo>(A);
			Foo anotherFooB = container.Resolve<Foo>(B);
			Assert.NotNull(anotherFooA);
			Assert.NotNull(anotherFooB);
			Assert.AreNotSame(anotherFooA, anotherFooB);
			Assert.AreNotSame(fooA, anotherFooA);
			Assert.AreNotSame(fooB, anotherFooB);
		}

		[Test]
		public void ResolveByInterface()
		{
			Container container = new Container();
			container.Bind<IFoo>().To<Foo>().AsTransient();

			Test(container);
		}

		[Test]
		public void ResolveByInterfaceWithId()
		{
			Container container = new Container();
			container.Bind<IFoo>().WithId(A).To<Foo>().AsTransient();
			container.Bind<IFoo>().WithId(B).To<Foo>().AsTransient();

			TestWithId(container);
		}

		[Test]
		public void ResolveFromFactory()
		{
			Container container = new Container();
			container.Bind<IFoo>().FromFactory<FooFactory>().AsTransient();

			Test(container);
		}

		[Test]
		public void ResolveFromFactoryWithId()
		{
			Container container = new Container();
			container.Bind<IFoo>().WithId(A).FromFactory<FooFactory>().AsTransient();
			container.Bind<IFoo>().WithId(B).FromFactory<FooFactory>().AsTransient();

			TestWithId(container);
		}

		[Test]
		public void ResolveFromFactoryMethod()
		{
			FooFactory factory = new FooFactory();

			Container container = new Container();
			container.Bind<IFoo>().FromFactoryMethod(factory.Create).AsTransient();

			Test(container);
		}

		[Test]
		public void ResolveFromFactoryMethodWithId()
		{
			FooFactory factory = new FooFactory();

			Container container = new Container();
			container.Bind<IFoo>().WithId(A).FromFactoryMethod(factory.Create).AsTransient();
			container.Bind<IFoo>().WithId(B).FromFactoryMethod(factory.Create).AsTransient();

			TestWithId(container);
		}

		private void Test(Container container)
		{
			Assert.IsNull(container.TryResolve<Foo>());

			IFoo foo = container.Resolve<IFoo>();
			Assert.NotNull(foo);

			IFoo anotherFoo = container.Resolve<IFoo>();
			Assert.NotNull(anotherFoo);
			Assert.AreNotSame(foo, anotherFoo);
		}

		private void TestWithId(Container container)
		{
			Assert.IsNull(container.TryResolve<IFoo>());
			Assert.IsNull(container.TryResolve<Foo>());
			Assert.IsNull(container.TryResolve<Foo>(A));
			Assert.IsNull(container.TryResolve<Foo>(B));

			IFoo fooA = container.Resolve<IFoo>(A);
			IFoo fooB = container.Resolve<IFoo>(B);
			Assert.NotNull(fooA);
			Assert.NotNull(fooB);
			Assert.AreNotSame(fooA, fooB);

			IFoo anotherFooA = container.Resolve<IFoo>(A);
			IFoo anotherFooB = container.Resolve<IFoo>(B);
			Assert.NotNull(anotherFooA);
			Assert.NotNull(anotherFooB);
			Assert.AreNotSame(anotherFooA, anotherFooB);
			Assert.AreNotSame(fooA, anotherFooA);
			Assert.AreNotSame(fooB, anotherFooB);
		}
	}

	[TestFixture]
	internal class ResolveFromContainerTests
	{
		private static readonly object A = "a";
		private static readonly object B = "b";

		[Test]
		public void ResolveFromResolve()
		{
			Foo instance = new Foo();

			Container container = new Container();
			container.Bind<Foo>().ToInstance(instance);
			container.Bind<IFoo>().FromResolve<Foo>();

			IFoo foo = container.Resolve<IFoo>();
			Assert.NotNull(foo);
			Assert.AreSame(foo, instance);
			Assert.AreSame(foo, container.Resolve<Foo>());
		}

		[Test]
		public void ResolveFromResolveWithId()
		{
			object a1 = "a1";
			object b1 = "b1";

			Foo instanceA = new Foo();
			Foo instanceB = new Foo();

			Container container = new Container();
			container.Bind<Foo>().WithId(A).ToInstance(instanceA);
			container.Bind<Foo>().WithId(B).ToInstance(instanceB);
			container.Bind<IFoo>().WithId(a1).FromResolve<Foo>(A);
			container.Bind<IFoo>().WithId(b1).FromResolve<Foo>(B);

			IFoo fooA = container.Resolve<IFoo>(a1);
			IFoo fooB = container.Resolve<IFoo>(b1);
			Assert.NotNull(fooA);
			Assert.NotNull(fooB);
			Assert.AreNotSame(fooA, fooB);
			Assert.AreSame(fooA, instanceA);
			Assert.AreSame(fooB, instanceB);
			Assert.AreSame(fooA, container.Resolve<Foo>(A));
			Assert.AreSame(fooB, container.Resolve<Foo>(B));
		}

		[Test]
		public void ResolveFromContainer()
		{
			Foo instance = new Foo();

			Container bindingContainer = new Container();
			bindingContainer.Bind<IFoo>().ToInstance(instance);

			Container container = new Container();
			container.Bind<IFoo>().FromContainer(bindingContainer);

			IFoo foo = container.Resolve<IFoo>();
			Assert.NotNull(foo);
			Assert.AreSame(foo, instance);
			Assert.AreSame(foo, bindingContainer.Resolve<IFoo>());
		}

		[Test]
		public void ResolveFromContainerWithId()
		{
			Foo instanceA = new Foo();
			Foo instanceB = new Foo();

			Container bindingContainer = new Container();
			bindingContainer.Bind<IFoo>().WithId(A).ToInstance(instanceA);
			bindingContainer.Bind<IFoo>().WithId(B).ToInstance(instanceB);

			Container container = new Container();
			container.Bind<IFoo>().WithId(A).FromContainer(bindingContainer);
			container.Bind<IFoo>().WithId(B).FromContainer(bindingContainer);

			IFoo fooA = container.Resolve<IFoo>(A);
			IFoo fooB = container.Resolve<IFoo>(B);
			Assert.NotNull(fooA);
			Assert.NotNull(fooB);
			Assert.AreNotSame(fooA, fooB);
			Assert.AreSame(fooA, instanceA);
			Assert.AreSame(fooB, instanceB);
			Assert.AreSame(fooA, bindingContainer.Resolve<IFoo>(A));
			Assert.AreSame(fooB, bindingContainer.Resolve<IFoo>(B));
		}
	}

	internal interface IFoo
	{
	}

	internal class Foo : IFoo
	{
	}

	internal class FooFactory : IFactory<IFoo>
	{
		public IFoo Create()
		{
			return new Foo();
		}
	}
}