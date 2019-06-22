using Chlorine.Exceptions;
using Chlorine.Factories;
using NUnit.Framework;

namespace Chlorine.Tests
{
	internal class ContainerResolveTests
	{
		private const string A = "A";
		private const string B = "B";

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
		private class MissingTests
		{
			[Test]
			public void MissingType_ExceptionThrown()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(instance);

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>());
			}

			[Test]
			public void MissingTypeTry_Null()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(instance);

				Assert.IsNull(container.TryResolve<IFoo>());
			}

			[Test]
			public void MissingId_ExceptionThrown()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).ToInstance(instance);

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>(B));
			}

			[Test]
			public void MissingIdTry_Null()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).ToInstance(instance);

				Assert.IsNull(container.TryResolve<IFoo>(B));
			}
		}

		[TestFixture]
		private class InstanceTests
		{
			[Test]
			public void Type_Resolve()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(instance);

				Assert.IsNull(container.TryResolve<IFoo>());
				Assert.AreSame(instance, container.Resolve<Foo>());
			}

			[Test]
			public void TypeWithId_Resolve()
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
			public void Interface_Resolve()
			{
				IFoo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().ToInstance(instance);

				Assert.IsNull(container.TryResolve<Foo>());
				Assert.AreSame(instance, container.Resolve<IFoo>());
			}

			[Test]
			public void InterfaceWithId_Resolve()
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
		private class SingletonTests
		{
			[Test]
			public void Type_Resolve()
			{
				Container container = new Container();
				container.Bind<Foo>().AsSingleton();

				Assert.IsNull(container.TryResolve<IFoo>());

				Foo foo = container.Resolve<Foo>();
				Assert.NotNull(foo);
				Assert.AreSame(foo, container.Resolve<Foo>());
			}

			[Test]
			public void TypeWithId_Resolve()
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
			public void Interface_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().To<Foo>().AsSingleton();

				TestResolveFrom(container);
			}

			[Test]
			public void InterfaceWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).To<Foo>().AsSingleton();
				container.Bind<IFoo>().WithId(B).To<Foo>().AsSingleton();

				TestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactory_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().FromFactory<FooFactory>().AsSingleton();

				TestResolveFrom(container);
			}

			[Test]
			public void FromFactoryWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactory<FooFactory>().AsSingleton();
				container.Bind<IFoo>().WithId(B).FromFactory<FooFactory>().AsSingleton();

				TestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactoryInstance_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().FromFactory(factory).AsSingleton();

				TestResolveFrom(container);
			}

			[Test]
			public void FromFactoryInstanceWithId_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactory(factory).AsSingleton();
				container.Bind<IFoo>().WithId(B).FromFactory(factory).AsSingleton();

				TestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactoryMethod_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().FromFactoryMethod(factory.Create).AsSingleton();

				TestResolveFrom(container);
			}

			[Test]
			public void FromFactoryMethodWithId_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactoryMethod(factory.Create).AsSingleton();
				container.Bind<IFoo>().WithId(B).FromFactoryMethod(factory.Create).AsSingleton();

				TestResolveWithIdFrom(container);
			}

			private void TestResolveFrom(Container container)
			{
				Assert.IsNull(container.TryResolve<Foo>());

				IFoo foo = container.Resolve<IFoo>();
				Assert.NotNull(foo);
				Assert.AreSame(foo, container.Resolve<IFoo>());
			}

			private void TestResolveWithIdFrom(Container container)
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
		private class TransientTests
		{
			[Test]
			public void Type_Resolve()
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
			public void TypeWithId_Resolve()
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
			public void Interface_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().To<Foo>().AsTransient();

				TestResolveFrom(container);
			}

			[Test]
			public void InterfaceWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).To<Foo>().AsTransient();
				container.Bind<IFoo>().WithId(B).To<Foo>().AsTransient();

				TestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactory_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().FromFactory<FooFactory>().AsTransient();

				TestResolveFrom(container);
			}

			[Test]
			public void FromFactoryWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactory<FooFactory>().AsTransient();
				container.Bind<IFoo>().WithId(B).FromFactory<FooFactory>().AsTransient();

				TestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactoryInstance_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().FromFactory(factory).AsTransient();

				TestResolveFrom(container);
			}

			[Test]
			public void FromFactoryInstanceWithId_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactory(factory).AsTransient();
				container.Bind<IFoo>().WithId(B).FromFactory(factory).AsTransient();

				TestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactoryMethod_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().FromFactoryMethod(factory.Create).AsTransient();

				TestResolveFrom(container);
			}

			[Test]
			public void FromFactoryMethodWithId_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactoryMethod(factory.Create).AsTransient();
				container.Bind<IFoo>().WithId(B).FromFactoryMethod(factory.Create).AsTransient();

				TestResolveWithIdFrom(container);
			}

			private void TestResolveFrom(Container container)
			{
				Assert.IsNull(container.TryResolve<Foo>());

				IFoo foo = container.Resolve<IFoo>();
				Assert.NotNull(foo);

				IFoo anotherFoo = container.Resolve<IFoo>();
				Assert.NotNull(anotherFoo);
				Assert.AreNotSame(foo, anotherFoo);
			}

			private void TestResolveWithIdFrom(Container container)
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
		private class FromContainerTests
		{
			[Test]
			public void FromResolve_Resolve()
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
			public void FromResolveWithId_Resolve()
			{
				const string a = "a";
				const string b = "b";

				Foo instanceA = new Foo();
				Foo instanceB = new Foo();

				Container container = new Container();
				container.Bind<Foo>().WithId(A).ToInstance(instanceA);
				container.Bind<Foo>().WithId(B).ToInstance(instanceB);
				container.Bind<IFoo>().WithId(a).FromResolve<Foo>(A);
				container.Bind<IFoo>().WithId(b).FromResolve<Foo>(B);

				IFoo fooA = container.Resolve<IFoo>(a);
				IFoo fooB = container.Resolve<IFoo>(b);
				Assert.NotNull(fooA);
				Assert.NotNull(fooB);
				Assert.AreNotSame(fooA, fooB);
				Assert.AreSame(fooA, instanceA);
				Assert.AreSame(fooB, instanceB);
				Assert.AreSame(fooA, container.Resolve<Foo>(A));
				Assert.AreSame(fooB, container.Resolve<Foo>(B));
			}

			[Test]
			public void FromContainer_Resolve()
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
			public void FromContainerWithId_Resolve()
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

			[Test]
			public void FromContainerResolve_Resolve()
			{
				Foo instance = new Foo();

				Container bindingContainer = new Container();
				bindingContainer.Bind<Foo>().ToInstance(instance);

				Container container = new Container();
				container.Bind<IFoo>().FromContainerResolve<Foo>(bindingContainer);

				IFoo foo = container.Resolve<IFoo>();
				Assert.NotNull(foo);
				Assert.AreSame(foo, instance);
				Assert.AreSame(foo, bindingContainer.Resolve<Foo>());
			}

			[Test]
			public void FromContainerResolveWithId_Resolve()
			{
				Foo instance = new Foo();

				Container bindingContainer = new Container();
				bindingContainer.Bind<Foo>().WithId(A).ToInstance(instance);

				Container container = new Container();
				container.Bind<IFoo>().WithId(B).FromContainerResolve<Foo>(bindingContainer, A);

				IFoo foo = container.Resolve<IFoo>(B);
				Assert.NotNull(foo);
				Assert.AreSame(foo, instance);
				Assert.AreSame(foo, bindingContainer.Resolve<Foo>(A));
			}
		}
	}
}