using System;
using NUnit.Framework;
using Pryo;

namespace Diver.Test.Rho
{
    [TestFixture]
    public class TestRhoCustomClass : TestCommon
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
            _reg.Register(new ClassBuilder<Foo>(_reg)
                .Methods
                    .Add<int,int,int>("Sum", (q,a,b) => q.Sum(a,b))
                    .Add<string,int,string>("MulString", (q,a,b) => q.MulString(a,b))
                .Class);

            var start = DateTime.Now;
            RhoRun(
@"
fun foo(type)
	a = type()
	assert(a.Num == 42)
	assert(a.Sum(a.Num, 3) == 42 + 3)
	assert(a.MulString(""foo"", 3) + ""bar"" == ""foofoofoobar"")
foo(Foo)
");
            var duration = DateTime.Now - start;
            WriteLine($"TotalTime {duration.TotalMilliseconds}ms");
            _continuation.Reset();

            start = DateTime.Now;
            _continuation.Reset();
	        _exec.Clear();
            _exec.Continue(_continuation);
	        var numOps = _exec.NumOps;
            WriteLine($"Just execution {numOps} ops: {(DateTime.Now - start).TotalMilliseconds}ms");
        }
    }
}