namespace Pyro.Test {
    using NUnit.Framework;

    [TestFixture]
    public class TestCoroutines
        : TestCommon {
        [Test]
        public void TestCoro1() {
            PiRun("{1} 'f# f&");
            AssertPop(1);

            // fun f()
            //      123
            // fun b(a)
            //      a()
            // b(f)
            PiRun("{123} 'f# {&} 'b# f b&", true);
            AssertPop(123);

            //            RhoRun(
            //@"fun f()
            //	123
            //fun b(a)
            //	a()
            //");
            //            AssertPop(123);
        }
    }
}
