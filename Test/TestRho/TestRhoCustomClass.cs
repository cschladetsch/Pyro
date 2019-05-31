using NUnit.Framework;

using Pyro;

namespace Diver.Test.Rho
{
    [TestFixture]
    public class TestRhoCustomClass
        : TestCommon
    {
        public class Foo
        {
            public int Num => 42;

            public int Sum(int a, int b)
            {
                return a + b;
            }

            public string MulString(string a, int b)
            {
                var str = "";
                while (b-- > 0)
                    str += a;
                return str;
            }
        }

        [Test]
        public void TestCustomClass()
        {
            // TODO: use reflection to find methods if not explicitly provided via ClassBuilder<> object
            _reg.Register(new ClassBuilder<Foo>(_reg)
                .Methods
                    .Add<int,int,int>("Sum", (q,a,b) => q.Sum(a,b))
                    .Add<string,int,string>("MulString", (q,a,b) => q.MulString(a,b))
                .Class);

            RhoRun(
@"
fun foo(type)
	a = type()
	assert(a.Num == 42)
	assert(a.Sum(a.Num, 3) == 42 + 3)
	assert(a.MulString(""foo"", 3) + ""bar"" == ""foofoofoobar"")
foo(Foo)
");
        }
    }
}