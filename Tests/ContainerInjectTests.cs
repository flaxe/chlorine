using Chlorine.Exceptions;
using NUnit.Framework;

namespace Chlorine.Tests
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
				public readonly Foo Foo;
				public readonly Bar Bar;

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
				public readonly Foo Foo;
				public readonly Bar Bar;

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
				public readonly Foo Foo;
				public readonly Bar Bar;

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
				public readonly Foo Foo;
				public readonly Bar Bar;

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
				public Foo Foo;
				public Bar Bar;
				public Qux QuxA;
				public Qux QuxB;
				public Baz BazA;

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
				Assert.NotNull(xyzzy);
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
		}

		[TestFixture]
		private class PropertyTests
		{
			private class Xyzzy
			{
				[Inject]
				public readonly Foo Foo;
				[Inject(Optional = true)]
				public readonly Bar Bar;
				[Inject(Id = A)]
				public readonly Qux QuxA;
				[Inject(Id = B, Optional = true)]
				public readonly Qux QuxB;
				[Inject(Id = A, Optional = true)]
				public readonly Baz Baz;
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
				private Foo _foo;

				[Inject(Optional = true)]
				public Bar Bar { set; get; }
				[Inject(Id = A)]
				public Qux QuxA { set; get; }
				[Inject(Id = B, Optional = true)]
				public Qux QuxB { set; get; }
				[Inject(Id = A, Optional = true)]
				public Baz Baz { set; get; }

				[Inject]
				public Foo Foo
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
				private Foo _foo;

				[Inject]
				public Foo Foo => _foo;
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
	}
}