using System.Security.AccessControl;
using NUnit.Framework;

namespace Diver.Test.Rho
{
    class Foo
    {
        public int Num => 42;

        public int Sum(int a, int b)
        {
            return a + b;
        }
    }

    [TestFixture]
    public class TestRhoCustomClass : TestCommon
    {
        [Test]
        public void TestCustomClass()
        {
            _reg.Register(new ClassBuilder<Foo>(_reg)
                .Methods
                    .Add<int,int,int>("Sum", (q,a,b) => q.Sum(a,b))
                .Class);
        }
    }
}