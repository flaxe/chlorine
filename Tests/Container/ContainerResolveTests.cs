using Carbone.Exceptions;
using NUnit.Framework;

namespace Carbone.Tests
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

		private class Bar : IFoo
		{
		}

		private class FooFactory : IFactory<IFoo>
		{
			public IFoo Create()
			{
				return new Foo();
			}
		}

		private class BarFactory : IFactory<IFoo>
		{
			public IFoo Create()
			{
				return new Bar();
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
			public void MissingTypeTry_IsNull()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(instance);

				Assert.IsNull(container.TryResolve<IFoo>());
			}

			[Test]
			public void MissingTypeWithId_ExceptionThrown()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).ToInstance(instance);

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>(B));
			}

			[Test]
			public void MissingTypeWithIdTry_IsNull()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).ToInstance(instance);

				Assert.IsNull(container.TryResolve<IFoo>(B));
			}

			[Test]
			public void MissingConditionalType_ExceptionThrown()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().When(context => false).ToInstance(instance);

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>());
			}

			[Test]
			public void MissingConditionalTypeTry_IsNull()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().When(context => false).ToInstance(instance);

				Assert.IsNull(container.TryResolve<IFoo>());
			}

			[Test]
			public void MissingConditionalTypeWithId_ExceptionThrown()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => false).ToInstance(instance);

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>(A));
			}

			[Test]
			public void MissingConditionalTypeWithIdTry_IsNull()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => false).ToInstance(instance);

				Assert.IsNull(container.TryResolve<IFoo>(A));
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

				Assert.AreSame(instanceA, container.Resolve<Foo>(A));
				Assert.AreSame(instanceB, container.Resolve<Foo>(B));
			}

			[Test]
			public void Interface_Resolve()
			{
				IFoo instance = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().ToInstance(instance);

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

				Assert.AreSame(instanceA, container.Resolve<IFoo>(A));
				Assert.AreSame(instanceB, container.Resolve<IFoo>(B));
			}

			[Test]
			public void ConditionalType_Resolve()
			{
				Foo instance = new Foo();
				Foo optional = new Foo();

				Container container = new Container();
				container.Bind<Foo>().When(context => !context.Optional).ToInstance(instance);
				container.Bind<Foo>().When(context => context.Optional).ToInstance(optional);

				Assert.AreSame(instance, container.Resolve<Foo>());
				Assert.AreSame(optional, container.TryResolve<Foo>());
			}

			[Test]
			public void ConditionalTypeWithId_Resolve()
			{
				Foo instance = new Foo();
				Foo optional = new Foo();

				Container container = new Container();
				container.Bind<Foo>().WithId(A).When(context => !context.Optional).ToInstance(instance);
				container.Bind<Foo>().WithId(B).When(context => context.Optional).ToInstance(optional);

				Assert.AreSame(instance, container.Resolve<Foo>(A));
				Assert.IsNull(container.TryResolve<Foo>(A));
				Assert.AreSame(optional, container.TryResolve<Foo>(B));
				Assert.Throws<ContainerException>(() => container.Resolve<Foo>(B));
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

				TestResolveFrom<Foo>(container);
			}

			[Test]
			public void TypeWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<Foo>().WithId(A).AsSingleton();
				container.Bind<Foo>().WithId(B).AsSingleton();

				TestResolveWithIdFrom<Foo>(container);
			}

			[Test]
			public void Interface_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().To<Foo>().AsSingleton();

				TestResolveFrom<IFoo>(container);
			}

			[Test]
			public void InterfaceWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).To<Foo>().AsSingleton();
				container.Bind<IFoo>().WithId(B).To<Foo>().AsSingleton();

				TestResolveWithIdFrom<IFoo>(container);
			}

			[Test]
			public void ConditionalInterface_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).To<Foo>().AsSingleton();
				container.Bind<IFoo>().When(context => context.Optional).To<Bar>().AsSingleton();

				ConditionalTestResolveFrom(container);
			}

			[Test]
			public void ConditionalInterfaceWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => !context.Optional).To<Foo>().AsSingleton();
				container.Bind<IFoo>().WithId(B).When(context => context.Optional).To<Bar>().AsSingleton();

				ConditionalTestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactory_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().FromFactory<FooFactory>().AsSingleton();

				TestResolveFrom<IFoo>(container);
			}

			[Test]
			public void FromFactoryWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactory<FooFactory>().AsSingleton();
				container.Bind<IFoo>().WithId(B).FromFactory<FooFactory>().AsSingleton();

				TestResolveWithIdFrom<IFoo>(container);
			}

			[Test]
			public void ConditionalFromFactory_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).FromFactory<FooFactory>().AsSingleton();
				container.Bind<IFoo>().When(context => context.Optional).FromFactory<BarFactory>().AsSingleton();

				ConditionalTestResolveFrom(container);
			}

			[Test]
			public void ConditionalFromFactoryWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => !context.Optional).FromFactory<FooFactory>().AsSingleton();
				container.Bind<IFoo>().WithId(B).When(context => context.Optional).FromFactory<BarFactory>().AsSingleton();

				ConditionalTestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactoryInstance_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().FromFactory(factory).AsSingleton();

				TestResolveFrom<IFoo>(container);
			}

			[Test]
			public void FromFactoryInstanceWithId_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactory(factory).AsSingleton();
				container.Bind<IFoo>().WithId(B).FromFactory(factory).AsSingleton();

				TestResolveWithIdFrom<IFoo>(container);
			}

			[Test]
			public void ConditionalFromFactoryInstance_Resolve()
			{
				FooFactory fooFactory = new FooFactory();
				BarFactory barFactory = new BarFactory();
				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).FromFactory(fooFactory).AsSingleton();
				container.Bind<IFoo>().When(context => context.Optional).FromFactory(barFactory).AsSingleton();

				ConditionalTestResolveFrom(container);
			}

			[Test]
			public void ConditionalFromFactoryInstanceWithId_Resolve()
			{
				FooFactory fooFactory = new FooFactory();
				BarFactory barFactory = new BarFactory();
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => !context.Optional).FromFactory(fooFactory).AsSingleton();
				container.Bind<IFoo>().WithId(B).When(context => context.Optional).FromFactory(barFactory).AsSingleton();

				ConditionalTestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactoryMethod_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().FromFactoryMethod(factory.Create).AsSingleton();

				TestResolveFrom<IFoo>(container);
			}

			[Test]
			public void FromFactoryMethodWithId_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactoryMethod(factory.Create).AsSingleton();
				container.Bind<IFoo>().WithId(B).FromFactoryMethod(factory.Create).AsSingleton();

				TestResolveWithIdFrom<IFoo>(container);
			}

			[Test]
			public void ConditionalFromFactoryMethod_Resolve()
			{
				FooFactory fooFactory = new FooFactory();
				BarFactory barFactory = new BarFactory();
				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).FromFactoryMethod(fooFactory.Create).AsSingleton();
				container.Bind<IFoo>().When(context => context.Optional).FromFactoryMethod(barFactory.Create).AsSingleton();

				ConditionalTestResolveFrom(container);
			}

			[Test]
			public void ConditionalFromFactoryMethodWithId_Resolve()
			{
				FooFactory fooFactory = new FooFactory();
				BarFactory barFactory = new BarFactory();
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => !context.Optional).FromFactoryMethod(fooFactory.Create).AsSingleton();
				container.Bind<IFoo>().WithId(B).When(context => context.Optional).FromFactoryMethod(barFactory.Create).AsSingleton();

				ConditionalTestResolveWithIdFrom(container);
			}

			private void TestResolveFrom<TFoo>(Container container) where TFoo : class
			{
				TFoo foo = container.Resolve<TFoo>();
				Assert.NotNull(foo);
				Assert.AreSame(foo, container.Resolve<TFoo>());
			}

			private void TestResolveWithIdFrom<TFoo>(Container container) where TFoo : class
			{
				TFoo fooA = container.Resolve<TFoo>(A);
				TFoo fooB = container.Resolve<TFoo>(B);
				Assert.NotNull(fooA);
				Assert.NotNull(fooB);
				Assert.AreNotSame(fooA, fooB);
				Assert.AreSame(fooA, container.Resolve<TFoo>(A));
				Assert.AreSame(fooB, container.Resolve<TFoo>(B));
			}

			private void ConditionalTestResolveFrom(Container container)
			{
				Assert.IsTrue(container.Resolve<IFoo>() is Foo);
				Assert.IsTrue(container.TryResolve<IFoo>() is Bar);
			}

			private void ConditionalTestResolveWithIdFrom(Container container)
			{
				Assert.IsTrue(container.Resolve<IFoo>(A) is Foo);
				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>(B));
				Assert.IsNull(container.TryResolve<IFoo>(A));
				Assert.IsTrue(container.TryResolve<IFoo>(B) is Bar);
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

				TestResolveFrom<Foo>(container);
			}

			[Test]
			public void TypeWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<Foo>().WithId(A).AsTransient();
				container.Bind<Foo>().WithId(B).AsTransient();

				TestResolveWithIdFrom<Foo>(container);
			}

			[Test]
			public void Interface_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().To<Foo>().AsTransient();

				TestResolveFrom<IFoo>(container);
			}

			[Test]
			public void InterfaceWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).To<Foo>().AsTransient();
				container.Bind<IFoo>().WithId(B).To<Foo>().AsTransient();

				TestResolveWithIdFrom<IFoo>(container);
			}

			[Test]
			public void ConditionalInterface_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).To<Foo>().AsTransient();
				container.Bind<IFoo>().When(context => context.Optional).To<Bar>().AsTransient();

				ConditionalTestResolveFrom(container);
			}

			[Test]
			public void ConditionalInterfaceWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => !context.Optional).To<Foo>().AsTransient();
				container.Bind<IFoo>().WithId(B).When(context => context.Optional).To<Bar>().AsTransient();

				ConditionalTestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactory_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().FromFactory<FooFactory>().AsTransient();

				TestResolveFrom<IFoo>(container);
			}

			[Test]
			public void FromFactoryWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactory<FooFactory>().AsTransient();
				container.Bind<IFoo>().WithId(B).FromFactory<FooFactory>().AsTransient();

				TestResolveWithIdFrom<IFoo>(container);
			}

			[Test]
			public void ConditionalFromFactory_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).FromFactory<FooFactory>().AsTransient();
				container.Bind<IFoo>().When(context => context.Optional).FromFactory<BarFactory>().AsTransient();

				ConditionalTestResolveFrom(container);
			}

			[Test]
			public void ConditionalFromFactoryWithId_Resolve()
			{
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => !context.Optional).FromFactory<FooFactory>().AsTransient();
				container.Bind<IFoo>().WithId(B).When(context => context.Optional).FromFactory<BarFactory>().AsTransient();

				ConditionalTestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactoryInstance_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().FromFactory(factory).AsTransient();

				TestResolveFrom<IFoo>(container);
			}

			[Test]
			public void FromFactoryInstanceWithId_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactory(factory).AsTransient();
				container.Bind<IFoo>().WithId(B).FromFactory(factory).AsTransient();

				TestResolveWithIdFrom<IFoo>(container);
			}

			[Test]
			public void ConditionalFromFactoryInstance_Resolve()
			{
				FooFactory fooFactory = new FooFactory();
				BarFactory barFactory = new BarFactory();
				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).FromFactory(fooFactory).AsTransient();
				container.Bind<IFoo>().When(context => context.Optional).FromFactory(barFactory).AsTransient();

				ConditionalTestResolveFrom(container);
			}

			[Test]
			public void ConditionalFromFactoryInstanceWithId_Resolve()
			{
				FooFactory fooFactory = new FooFactory();
				BarFactory barFactory = new BarFactory();
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => !context.Optional).FromFactory(fooFactory).AsTransient();
				container.Bind<IFoo>().WithId(B).When(context => context.Optional).FromFactory(barFactory).AsTransient();

				ConditionalTestResolveWithIdFrom(container);
			}

			[Test]
			public void FromFactoryMethod_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().FromFactoryMethod(factory.Create).AsTransient();

				TestResolveFrom<IFoo>(container);
			}

			[Test]
			public void FromFactoryMethodWithId_Resolve()
			{
				FooFactory factory = new FooFactory();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).FromFactoryMethod(factory.Create).AsTransient();
				container.Bind<IFoo>().WithId(B).FromFactoryMethod(factory.Create).AsTransient();

				TestResolveWithIdFrom<IFoo>(container);
			}

			[Test]
			public void ConditionalFromFactoryMethod_Resolve()
			{
				FooFactory fooFactory = new FooFactory();
				BarFactory barFactory = new BarFactory();
				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).FromFactoryMethod(fooFactory.Create).AsTransient();
				container.Bind<IFoo>().When(context => context.Optional).FromFactoryMethod(barFactory.Create).AsTransient();

				ConditionalTestResolveFrom(container);
			}

			[Test]
			public void ConditionalFromFactoryMethodWithId_Resolve()
			{
				FooFactory fooFactory = new FooFactory();
				BarFactory barFactory = new BarFactory();
				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => !context.Optional).FromFactoryMethod(fooFactory.Create).AsTransient();
				container.Bind<IFoo>().WithId(B).When(context => context.Optional).FromFactoryMethod(barFactory.Create).AsTransient();

				ConditionalTestResolveWithIdFrom(container);
			}

			private void TestResolveFrom<TFoo>(Container container) where TFoo : class
			{
				TFoo foo = container.Resolve<TFoo>();
				Assert.IsNotNull(foo);
				Assert.AreNotSame(foo, container.Resolve<TFoo>());
			}

			private void TestResolveWithIdFrom<TFoo>(Container container) where TFoo : class
			{
				TFoo fooA = container.Resolve<TFoo>(A);
				TFoo fooB = container.Resolve<TFoo>(B);
				Assert.IsNotNull(fooA);
				Assert.IsNotNull(fooB);
				Assert.AreNotSame(fooA, fooB);
				Assert.AreNotSame(fooA, container.Resolve<TFoo>(A));
				Assert.AreNotSame(fooB, container.Resolve<TFoo>(B));
			}

			private void ConditionalTestResolveFrom(Container container)
			{
				Assert.IsTrue(container.Resolve<IFoo>() is Foo);
				Assert.IsTrue(container.TryResolve<IFoo>() is Bar);
			}

			private void ConditionalTestResolveWithIdFrom(Container container)
			{
				Assert.IsTrue(container.Resolve<IFoo>(A) is Foo);
				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>(B));
				Assert.IsNull(container.TryResolve<IFoo>(A));
				Assert.IsTrue(container.TryResolve<IFoo>(B) is Bar);
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

				Assert.AreSame(instance, container.Resolve<IFoo>());
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

				Assert.AreSame(instanceA, container.Resolve<IFoo>(a));
				Assert.AreSame(instanceB, container.Resolve<IFoo>(b));
			}

			[Test]
			public void ConditionalFromResolve_Resolve()
			{
				Foo instance = new Foo();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(instance);
				container.Bind<IFoo>().When(context => !context.Optional).FromResolve<Foo>();

				Assert.AreSame(instance, container.Resolve<IFoo>());
				Assert.IsNull(container.TryResolve<IFoo>());
			}

			[Test]
			public void ConditionalFromResolveWithId_Resolve()
			{
				const string a = "a";
				const string b = "b";

				Foo instanceA = new Foo();
				Foo instanceB = new Foo();

				Container container = new Container();
				container.Bind<Foo>().WithId(A).ToInstance(instanceA);
				container.Bind<Foo>().WithId(B).ToInstance(instanceB);
				container.Bind<IFoo>().WithId(a).When(context => !context.Optional).FromResolve<Foo>(A);
				container.Bind<IFoo>().WithId(b).When(context => context.Optional).FromResolve<Foo>(B);

				Assert.AreSame(instanceA, container.Resolve<IFoo>(a));
				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>(b));
				Assert.IsNull(container.TryResolve<IFoo>(a));
				Assert.AreSame(instanceB, container.TryResolve<IFoo>(b));
			}

			[Test]
			public void FromContainer_Resolve()
			{
				Foo instance = new Foo();

				Container bindingContainer = new Container();
				bindingContainer.Bind<IFoo>().ToInstance(instance);

				Container container = new Container();
				container.Bind<IFoo>().FromContainer(bindingContainer);

				Assert.AreSame(instance, container.Resolve<IFoo>());
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

				Assert.AreSame(instanceA, container.Resolve<IFoo>(A));
				Assert.AreSame(instanceB, container.Resolve<IFoo>(B));
			}

			[Test]
			public void ConditionalFromContainer_Resolve()
			{
				Foo instance = new Foo();

				Container bindingContainer = new Container();
				bindingContainer.Bind<IFoo>().ToInstance(instance);

				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).FromContainer(bindingContainer);

				Assert.AreSame(instance, container.Resolve<IFoo>());
				Assert.IsNull(container.TryResolve<IFoo>());
			}

			[Test]
			public void ConditionalFromContainerWithId_Resolve()
			{
				Foo instanceA = new Foo();
				Foo instanceB = new Foo();

				Container bindingContainer = new Container();
				bindingContainer.Bind<IFoo>().WithId(A).ToInstance(instanceA);
				bindingContainer.Bind<IFoo>().WithId(B).ToInstance(instanceB);

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).When(context => !context.Optional).FromContainer(bindingContainer);
				container.Bind<IFoo>().WithId(B).When(context => context.Optional).FromContainer(bindingContainer);

				Assert.AreSame(instanceA, container.Resolve<IFoo>(A));
				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>(B));
				Assert.IsNull(container.TryResolve<IFoo>(A));
				Assert.AreSame(instanceB, container.TryResolve<IFoo>(B));
			}

			[Test]
			public void FromContainerResolve_Resolve()
			{
				Foo instance = new Foo();

				Container bindingContainer = new Container();
				bindingContainer.Bind<Foo>().ToInstance(instance);

				Container container = new Container();
				container.Bind<IFoo>().FromContainerResolve<Foo>(bindingContainer);

				Assert.AreSame(instance, container.Resolve<IFoo>());
			}

			[Test]
			public void FromContainerResolveWithId_Resolve()
			{
				Foo instance = new Foo();

				Container bindingContainer = new Container();
				bindingContainer.Bind<Foo>().WithId(A).ToInstance(instance);

				Container container = new Container();
				container.Bind<IFoo>().WithId(B).FromContainerResolve<Foo>(bindingContainer, A);

				Assert.AreSame(instance, container.Resolve<IFoo>(B));
			}

			[Test]
			public void ConditionalFromContainerResolve_Resolve()
			{
				Foo instance = new Foo();

				Container bindingContainer = new Container();
				bindingContainer.Bind<Foo>().ToInstance(instance);

				Container container = new Container();
				container.Bind<IFoo>().When(context => !context.Optional).FromContainerResolve<Foo>(bindingContainer);

				Assert.AreSame(instance, container.Resolve<IFoo>());
				Assert.IsNull(container.TryResolve<IFoo>());
			}

			[Test]
			public void ConditionalFromContainerResolveWithId_Resolve()
			{
				Foo instance = new Foo();

				Container bindingContainer = new Container();
				bindingContainer.Bind<Foo>().WithId(A).ToInstance(instance);

				Container container = new Container();
				container.Bind<IFoo>().WithId(B).When(context => !context.Optional).FromContainerResolve<Foo>(bindingContainer, A);

				Assert.AreSame(instance, container.Resolve<IFoo>(B));
				Assert.IsNull(container.TryResolve<IFoo>(B));
			}
		}

		[TestFixture]
		private class FromSubContainerTests
		{
			[Test]
			public void ParentBinding_Resolve()
			{
				IFoo foo = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().ToInstance(foo);

				Container subContainer = container.CreateSubContainer();
				Assert.AreSame(foo, subContainer.Resolve<IFoo>());
			}

			[Test]
			public void ChildrenBinding_ExceptionThrown()
			{
				IFoo foo = new Foo();

				Container container = new Container();

				Container subContainer = container.CreateSubContainer();
				subContainer.Bind<IFoo>().ToInstance(foo);

				Assert.Throws<ContainerException>(() => container.Resolve<IFoo>());
			}

			[Test]
			public void OverrideBinding_Resolve()
			{
				IFoo foo1 = new Foo();
				IFoo foo2 = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().ToInstance(foo1);

				Container subContainer = container.CreateSubContainer();
				subContainer.Bind<IFoo>().ToInstance(foo2);

				Assert.AreSame(foo1, container.Resolve<IFoo>());
				Assert.AreSame(foo2, subContainer.Resolve<IFoo>());
			}

			[Test]
			public void OverrideWithIdBinding_Resolve()
			{
				IFoo foo1 = new Foo();
				IFoo foo2 = new Foo();

				Container container = new Container();
				container.Bind<IFoo>().WithId(A).ToInstance(foo1);

				Container subContainer = container.CreateSubContainer();
				subContainer.Bind<IFoo>().WithId(A).ToInstance(foo2);

				Assert.AreSame(foo1, container.Resolve<IFoo>(A));
				Assert.AreSame(foo2, subContainer.Resolve<IFoo>(A));
			}
		}
	}
}