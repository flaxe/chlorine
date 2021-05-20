using Carbone.Exceptions;
using NUnit.Framework;

namespace Carbone.Tests
{
	internal class ContainerInjectTests
	{
		private const string A = "A";
		private const string B = "B";

		private class Foo
		{
		}

		private class Bar
		{
		}

		private class Qux
		{
		}

		private class Baz
		{
		}

		[TestFixture]
		private class ContructorTests
		{
			private class Xyzzy
			{
				public readonly Foo Foo;
				public readonly Bar Bar;
				public readonly Qux QuxA;
				public readonly Qux QuxB;
				public readonly Baz BazA;

				public Xyzzy(
						Foo foo,
						[Inject(Optional = true)]Bar bar,
						[Inject(Id = A)]Qux quxA,
						[Inject(Id = B, Optional = true)]Qux quxB,
						[Inject(Id = A, Optional = true)]Baz bazA)
				{
					Foo = foo;
					Bar = bar;
					QuxA = quxA;
					QuxB = quxB;
					BazA = bazA;
				}
			}

			[Test]
			public void Constructor_Instantiate()
			{
				Foo foo = new Foo();
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Xyzzy xyzzy = container.Instantiate<Xyzzy>();
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.Foo, foo);
				Assert.IsNull(xyzzy.Bar);
				Assert.AreSame(xyzzy.QuxA, quxA);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.BazA);
			}

			[Test]
			public void ConstructorMissingType_ExceptionThrown()
			{
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy>());
			}

			[Test]
			public void ConstructorMissingId_ExceptionThrown()
			{
				Foo foo = new Foo();
				Qux quxB = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(B).ToInstance(quxB);

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy>());
			}

			private class Zzyzx
			{
				public readonly Foo? Foo;
				public readonly Bar? Bar;

				public Zzyzx(Foo foo)
				{
					Foo = foo;
				}

				public Zzyzx(Bar bar)
				{
					Bar = bar;
				}
			}

			[Test]
			public void MultipleConstructor_ExceptionThrown()
			{
				Foo foo = new Foo();
				Bar bar = new Bar();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Bar>().ToInstance(bar);

				Assert.Throws<InjectException>(() => container.Instantiate<Zzyzx>());
			}

			private class Waldo
			{
				public readonly Foo? Foo;
				public readonly Bar? Bar;

				[Inject]
				private Waldo(Foo foo)
				{
					Foo = foo;
				}

				public Waldo(Bar bar)
				{
					Bar = bar;
				}
			}

			[Test]
			public void SingleConstructorWithAttribute_Instantiate()
			{
				Foo foo = new Foo();
				Bar bar = new Bar();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Bar>().ToInstance(bar);

				Waldo waldo = container.Instantiate<Waldo>();
				Assert.NotNull(waldo);
				Assert.AreSame(waldo.Foo, foo);
				Assert.IsNull(waldo.Bar);
			}

			private class Barney
			{
				public readonly Foo? Foo;
				public readonly Bar? Bar;

				[Inject]
				private Barney(Foo foo)
				{
					Foo = foo;
				}

				[Inject]
				public Barney(Bar bar)
				{
					Bar = bar;
				}
			}

			[Test]
			public void MultipleConstructorWithAttribute_ExceptionThrown()
			{
				Foo foo = new Foo();
				Bar bar = new Bar();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Bar>().ToInstance(bar);

				Assert.Throws<InjectException>(() => container.Instantiate<Barney>());
			}

			private class Bazola
			{
				public readonly Foo? Foo;
				public readonly Bar? Bar;

				public Bazola(Foo foo)
				{
					Foo = foo;
				}

				private Bazola(Bar bar)
				{
					Bar = bar;
				}
			}

			[Test]
			public void SinglePublicConstructor_Instantiate()
			{
				Foo foo = new Foo();
				Bar bar = new Bar();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Bar>().ToInstance(bar);

				Bazola bazola = container.Instantiate<Bazola>();
				Assert.NotNull(bazola);
				Assert.AreSame(bazola.Foo, foo);
				Assert.IsNull(bazola.Bar);
			}

			private abstract class Grunt
			{
				public readonly Foo Foo;

				public Grunt(Foo foo)
				{
					Foo = foo;
				}
			}

			[Test]
			public void AbstractConstructor_ExceptionThrown()
			{
				Foo foo = new Foo();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);

				Assert.Throws<InjectException>(() => container.Instantiate<Grunt>());
			}

			private enum Wombat
			{
				Foo = 1,
				Bar = 2
			}

			[Test]
			public void EnumConstructor_ExceptionThrown()
			{
				Container container = new Container();

				Assert.Throws<InjectException>(() => container.Instantiate<Wombat>());
			}

			[Test]
			public void ArrayConstructor_ExceptionThrown()
			{
				Container container = new Container();

				Assert.Throws<InjectException>(() => container.Instantiate<Wombat[]>());
			}
		}

		[TestFixture]
		private class MethodTests
		{
			private class Xyzzy
			{
				public Foo? Foo;
				public Bar? Bar;
				public Qux? QuxA;
				public Qux? QuxB;
				public Baz? BazA;

				[Inject]
				public void InitFooBar(
						Foo foo,
						[Inject(Optional = true)]Bar bar)
				{
					Foo = foo;
					Bar = bar;
				}

				[Inject]
				public void InitQuxBaz(
						[Inject(Id = A)]Qux quxA,
						[Inject(Id = B, Optional = true)]Qux quxB,
						[Inject(Id = A, Optional = true)]Baz bazA)
				{
					QuxA = quxA;
					QuxB = quxB;
					BazA = bazA;
				}
			}

			[Test]
			public void Method_Instantiate()
			{
				Foo foo = new Foo();
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Xyzzy xyzzy = container.Instantiate<Xyzzy>();
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.Foo, foo);
				Assert.IsNull(xyzzy.Bar);
				Assert.AreSame(xyzzy.QuxA, quxA);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.BazA);
			}

			[Test]
			public void Method_Inject()
			{
				Foo foo = new Foo();
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Xyzzy xyzzy = new Xyzzy();
				container.Inject(xyzzy);
				Assert.AreSame(xyzzy.Foo, foo);
				Assert.IsNull(xyzzy.Bar);
				Assert.AreSame(xyzzy.QuxA, quxA);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.BazA);
			}

			[Test]
			public void MethodMissingType_ExceptionThrown()
			{
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy>());
			}

			[Test]
			public void MethodMissingId_ExceptionThrown()
			{
				Foo foo = new Foo();
				Qux quxB = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(B).ToInstance(quxB);

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy>());
			}

			private class Waldo
			{
				public Foo? Foo;

				[Inject]
				private void Init()
				{
					Foo = new Foo();
				}
			}

			[Test]
			public void EmptyMethod_Inject()
			{
				Container container = new Container();

				Waldo waldo = new Waldo();
				container.Inject(waldo);
				Assert.IsNotNull(waldo.Foo);
			}

			private class Barney : Waldo
			{
				public Qux? Qux;

				[Inject]
				private void Init()
				{
					Qux = new Qux();
				}
			}

			[Test]
			public void HiddenEmptyMethod_Inject()
			{
				Container container = new Container();

				Barney barney = new Barney();
				container.Inject(barney);
				Assert.IsNotNull(barney.Foo);
				Assert.IsNotNull(barney.Qux);
			}
		}

		[TestFixture]
		private class PropertyTests
		{
			private class Xyzzy
			{
				[Inject]
				public readonly Foo? Foo = default;
				[Inject(Optional = true)]
				public readonly Bar? Bar = default;
				[Inject(Id = A)]
				public readonly Qux? QuxA = default;
				[Inject(Id = B, Optional = true)]
				public readonly Qux? QuxB = default;
				[Inject(Id = A, Optional = true)]
				public readonly Baz? Baz = default;
			}

			[Test]
			public void Field_Instantiate()
			{
				Foo foo = new Foo();
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Xyzzy xyzzy = container.Instantiate<Xyzzy>();
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.Foo, foo);
				Assert.IsNull(xyzzy.Bar);
				Assert.AreSame(xyzzy.QuxA, quxA);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.Baz);
			}

			[Test]
			public void Field_Inject()
			{
				Foo foo = new Foo();
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Xyzzy xyzzy = new Xyzzy();
				container.Inject(xyzzy);
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.Foo, foo);
				Assert.IsNull(xyzzy.Bar);
				Assert.AreSame(xyzzy.QuxA, quxA);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.QuxB);
				Assert.IsNull(xyzzy.Baz);
			}

			[Test]
			public void FieldMissingType_ExceptionThrown()
			{
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy>());
			}

			[Test]
			public void FieldMissingId_ExceptionThrown()
			{
				Foo foo = new Foo();
				Qux quxB = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(B).ToInstance(quxB);

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy>());
			}

			private class Zzyzx
			{
				private Foo? _foo;

				[Inject(Optional = true)]
				public Bar? Bar { set; get; }
				[Inject(Id = A)]
				public Qux? QuxA { set; get; }
				[Inject(Id = B, Optional = true)]
				public Qux? QuxB { set; get; }
				[Inject(Id = A, Optional = true)]
				public Baz? Baz { set; get; }

				[Inject]
				public Foo? Foo
				{
					set { _foo = value; }
					get { return _foo; }
				}
			}

			[Test]
			public void Property_Instantiate()
			{
				Foo foo = new Foo();
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Zzyzx zzyzx = container.Instantiate<Zzyzx>();
				Assert.NotNull(zzyzx);
				Assert.AreSame(zzyzx.Foo, foo);
				Assert.IsNull(zzyzx.Bar);
				Assert.AreSame(zzyzx.QuxA, quxA);
				Assert.IsNull(zzyzx.QuxB);
				Assert.IsNull(zzyzx.QuxB);
				Assert.IsNull(zzyzx.Baz);
			}

			[Test]
			public void Property_Inject()
			{
				Foo foo = new Foo();
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Zzyzx zzyzx = new Zzyzx();
				container.Inject(zzyzx);
				Assert.NotNull(zzyzx);
				Assert.AreSame(zzyzx.Foo, foo);
				Assert.IsNull(zzyzx.Bar);
				Assert.AreSame(zzyzx.QuxA, quxA);
				Assert.IsNull(zzyzx.QuxB);
				Assert.IsNull(zzyzx.QuxB);
				Assert.IsNull(zzyzx.Baz);
			}

			[Test]
			public void PropertyMissingType_ExceptionThrown()
			{
				Qux quxA = new Qux();

				Container container = new Container();
				container.Bind<Qux>().WithId(A).ToInstance(quxA);

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy>());
			}

			[Test]
			public void PropertyMissingId_ExceptionThrown()
			{
				Foo foo = new Foo();
				Qux quxB = new Qux();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);
				container.Bind<Qux>().WithId(B).ToInstance(quxB);

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy>());
			}

			private class Waldo
			{
				private Foo? _foo = default;

				[Inject]
				public Foo? Foo => _foo;
			}

			[Test]
			public void ReadonlyProperty_ExceptionThrown()
			{
				Foo foo = new Foo();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo);

				Assert.Throws<InjectException>(() => container.Instantiate<Waldo>());
			}
		}

		[TestFixture]
		private class ArgumentsTests
		{
			private class Xyzzy
			{
				public readonly Foo Foo;
				public readonly Bar Bar;

				public Xyzzy(Foo foo, Bar bar)
				{
					Foo = foo;
					Bar = bar;
				}
			}

			[Test]
			public void ConstructorArguments_Instantiate()
			{
				Foo foo = new Foo();
				Bar bar = new Bar();

				Container container = new Container();

				Xyzzy xyzzy = container.Instantiate<Xyzzy, Foo, Bar>(foo, bar);
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.Foo, foo);
				Assert.AreSame(xyzzy.Bar, bar);
			}

			[Test]
			public void ConstructorArgumentsOverride_Instantiate()
			{
				Foo foo1 = new Foo();
				Foo foo2 = new Foo();
				Bar bar = new Bar();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo1);

				Xyzzy xyzzy = container.Instantiate<Xyzzy, Foo, Bar>(foo2, bar);
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.Foo, foo2);
				Assert.AreSame(xyzzy.Bar, bar);
			}

			[Test]
			public void MissingConstructorArguments_ExceptionThrown()
			{
				Foo foo = new Foo();

				Container container = new Container();

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy, Foo>(foo));
			}

			private class Waldo
			{
				public readonly Foo Foo1;
				public readonly Foo Foo2;
				public readonly Foo Foo3;

				public Waldo(Foo foo1, Foo foo2, Foo foo3)
				{
					Foo1 = foo1;
					Foo2 = foo2;
					Foo3 = foo3;
				}
			}

			[Test]
			public void ConstructorArgumentsOrder_Instantiate()
			{
				Foo foo1 = new Foo();
				Foo foo2 = new Foo();
				Foo foo3 = new Foo();

				Container container = new Container();

				Waldo waldo = container.Instantiate<Waldo, Foo, Foo, Foo>(foo1, foo2, foo3);
				Assert.NotNull(waldo);
				Assert.AreSame(waldo.Foo1, foo1);
				Assert.AreSame(waldo.Foo2, foo2);
				Assert.AreSame(waldo.Foo3, foo3);
			}

			private class Zzyzx
			{
				public Foo? Foo;
				public Bar? Bar;

				[Inject]
				public void Inject(Foo foo, Bar bar)
				{
					Foo = foo;
					Bar = bar;
				}
			}

			[Test]
			public void MethodArguments_Inject()
			{
				Foo foo = new Foo();
				Bar bar = new Bar();

				Zzyzx zzyzx = new Zzyzx();

				Container container = new Container();
				container.Inject(zzyzx, foo, bar);

				Assert.AreSame(zzyzx.Foo, foo);
				Assert.AreSame(zzyzx.Bar, bar);
			}

			[Test]
			public void MethodArgumentsOverride_Inject()
			{
				Foo foo1 = new Foo();
				Foo foo2 = new Foo();
				Bar bar = new Bar();

				Zzyzx zzyzx = new Zzyzx();

				Container container = new Container();
				container.Bind<Foo>().ToInstance(foo1);
				container.Inject(zzyzx, foo2, bar);

				Assert.AreSame(zzyzx.Foo, foo2);
				Assert.AreSame(zzyzx.Bar, bar);
			}

			[Test]
			public void MissingMethodArguments_ExceptionThrown()
			{
				Foo foo = new Foo();

				Zzyzx zzyzx = new Zzyzx();

				Container container = new Container();

				Assert.Throws<InjectException>(() => container.Inject(zzyzx, foo));
			}

			private class Barney
			{
				public Foo? Foo1;
				public Foo? Foo2;
				public Foo? Foo3;

				[Inject]
				public void Inject(Foo foo1, Foo foo2, Foo foo3)
				{
					Foo1 = foo1;
					Foo2 = foo2;
					Foo3 = foo3;
				}
			}

			[Test]
			public void MethodArgumentsOrder_Inject()
			{
				Foo foo1 = new Foo();
				Foo foo2 = new Foo();
				Foo foo3 = new Foo();

				Barney barney = new Barney();

				Container container = new Container();
				container.Inject(barney, foo1, foo2, foo3);

				Assert.AreSame(barney.Foo1, foo1);
				Assert.AreSame(barney.Foo2, foo2);
				Assert.AreSame(barney.Foo3, foo3);
			}
		}

		[TestFixture]
		private class DependencyTests
		{
			private class Xyzzy
			{
				[Inject]
				public readonly Waldo? Waldo = default;
			}

			private class Waldo
			{
				[Inject]
				public readonly Barney? Barney = default;
			}

			private class Barney
			{
				[Inject]
				public readonly Xyzzy? Xyzzy = default;
			}

			[Test]
			public void CircularFieldDependency_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<Xyzzy>().AsSingleton();
				container.Bind<Waldo>().AsSingleton();
				container.Bind<Barney>().AsSingleton();

				Assert.Throws<InjectException>(() => container.Instantiate<Xyzzy>());
			}

			private class Bazola
			{
				public readonly Grunt Grunt;

				public Bazola(Grunt grunt)
				{
					Grunt = grunt;
				}
			}

			private class Grunt
			{
				public readonly Wombat Wombat;

				public Grunt(Wombat wombat)
				{
					Wombat = wombat;
				}
			}

			private class Wombat
			{
				public readonly Bazola Bazola;

				public Wombat(Bazola bazola)
				{
					Bazola = bazola;
				}
			}

			[Test]
			public void CircularConstructorDependency_ExceptionThrown()
			{
				Container container = new Container();
				container.Bind<Bazola>().AsSingleton();
				container.Bind<Grunt>().AsSingleton();
				container.Bind<Wombat>().AsSingleton();

				Assert.Throws<InjectException>(() => container.Instantiate<Bazola>());
			}
		}

		[TestFixture]
		private class ConditionalTests
		{
			private class Xyzzy
			{
				[Inject]
				public readonly Foo? FooA = default;
				[Inject(Optional = true)]
				public readonly Foo? FooB = default;
				[Inject(Id = A)]
				public readonly Bar? BarA = default;
				[Inject(Id = A, Optional = true)]
				public readonly Bar? BarB = default;
			}

			[Test]
			public void Instantiate_AreSame()
			{
				Foo foo = new Foo();
				Bar bar = new Bar();

				Container container = new Container();
				container.Bind<Foo>().When(context => !context.Optional).ToInstance(foo);
				container.Bind<Bar>().WithId(A).When(context => !context.Optional).ToInstance(bar);

				Xyzzy xyzzy = container.Instantiate<Xyzzy>();
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.FooA, foo);
				Assert.IsNull(xyzzy.FooB);
				Assert.AreSame(xyzzy.BarA, bar);
				Assert.IsNull(xyzzy.BarB);
			}

			[Test]
			public void Inject_AreSame()
			{
				Foo foo = new Foo();
				Bar bar = new Bar();

				Container container = new Container();
				container.Bind<Foo>().When(context => !context.Optional).ToInstance(foo);
				container.Bind<Bar>().WithId(A).When(context => !context.Optional).ToInstance(bar);

				Xyzzy xyzzy = new Xyzzy();
				container.Inject(xyzzy);
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.FooA, foo);
				Assert.IsNull(xyzzy.FooB);
				Assert.AreSame(xyzzy.BarA, bar);
				Assert.IsNull(xyzzy.BarB);
			}
		}

		[TestFixture]
		private class WhenInjectedIntoTests
		{
			private class Xyzzy
			{
				[Inject]
				public readonly Foo? Foo = default;
				[Inject(Id = A)]
				public readonly Bar? Bar = default;
			}

			private class Zzyzx
			{
				[Inject]
				public readonly Foo? Foo = default;
				[Inject(Id = A)]
				public readonly Bar? Bar = default;
			}

			[Test]
			public void Instantiate_AreSame()
			{
				Foo fooX = new Foo();
				Foo fooZ = new Foo();
				Bar barX = new Bar();
				Bar barZ = new Bar();

				Container container = new Container();
				container.Bind<Foo>().WhenInjectInto<Xyzzy>().ToInstance(fooX);
				container.Bind<Foo>().WhenInjectInto<Zzyzx>().ToInstance(fooZ);
				container.Bind<Bar>().WithId(A).WhenInjectInto<Xyzzy>().ToInstance(barX);
				container.Bind<Bar>().WithId(A).WhenInjectInto<Zzyzx>().ToInstance(barZ);

				Xyzzy xyzzy = container.Instantiate<Xyzzy>();
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.Foo, fooX);
				Assert.AreSame(xyzzy.Bar, barX);

				Zzyzx zzyzx = container.Instantiate<Zzyzx>();
				Assert.NotNull(zzyzx);
				Assert.AreSame(zzyzx.Foo, fooZ);
				Assert.AreSame(zzyzx.Bar, barZ);
			}

			[Test]
			public void Inject_AreSame()
			{
				Foo fooX = new Foo();
				Foo fooZ = new Foo();
				Bar barX = new Bar();
				Bar barZ = new Bar();

				Container container = new Container();
				container.Bind<Foo>().WhenInjectInto<Xyzzy>().ToInstance(fooX);
				container.Bind<Foo>().WhenInjectInto<Zzyzx>().ToInstance(fooZ);
				container.Bind<Bar>().WithId(A).WhenInjectInto<Xyzzy>().ToInstance(barX);
				container.Bind<Bar>().WithId(A).WhenInjectInto<Zzyzx>().ToInstance(barZ);

				Xyzzy xyzzy = new Xyzzy();
				container.Inject(xyzzy);
				Assert.NotNull(xyzzy);
				Assert.AreSame(xyzzy.Foo, fooX);
				Assert.AreSame(xyzzy.Bar, barX);

				Zzyzx zzyzx = new Zzyzx();
				container.Inject(zzyzx);
				Assert.NotNull(zzyzx);
				Assert.AreSame(zzyzx.Foo, fooZ);
				Assert.AreSame(zzyzx.Bar, barZ);
			}
		}
	}
}