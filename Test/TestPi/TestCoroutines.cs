using NUnit.Framework;

namespace Pyro.Test {
    [TestFixture]
    public class TestCoroutines
        : TestCommon {
        [Test]
        public void TestCoro1() {
            // fun f()
            //      123
            // fun b(a)
            //      a()
            // b(f)
            PiRun("{123} 'f# {&} 'b# f b&");
            AssertPop(123);

            PiRun("{1} 'f# f& f& + ");
            AssertPop(2);

            PiRun("{1} &");
            AssertPop(1);

            PiRun("{1} 'f# f&");
            AssertPop(1);

            PiRun("1 2 +");
            AssertPop(3);

            PiRun("1 2 {+} &");
            AssertPop(3);

            PiRun("1 2 { + } & 3 == assert");
        }
    }
}