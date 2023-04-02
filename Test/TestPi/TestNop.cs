using NUnit.Framework;

namespace Pyro.Test {
    [TestFixture]
    public class TestNop
        : TestCommon {
        [Test]
        public void TestNopOperation() {
            PiRun("nop");
            AssertEmpty();
        }

        [Test]
        public void TestNopOperationInExpression() {
            PiRun("1 2 nop +");
            AssertPop(3);
        }

        [Test]
        public void TestNopContinuation() {
            PiRun("{ nop } &");
            AssertEmpty();
        }
    }
}

