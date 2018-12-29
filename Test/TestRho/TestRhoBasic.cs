using NUnit.Framework;

namespace Diver.Test.Rho
{
    [TestFixture]
    public class TestRhoBasic : TestCommon
    {
        [Test]
        public void TestBoolean()
        {
            Ensure("true");
            Ensure("!false");
            Ensure("!!true");
            Ensure("true == true");
            Ensure("true || false");
            Ensure("true != false");
            Ensure("true ^ false");
            Ensure("!(true && false)");
            Ensure("(true || false) ^ false");
            Ensure("!!(true ^ false)");
        }

        private void Ensure(string text)
        {
            RunRho($"assert({text})");
        }

        [Test]
        public void TestArithmetic()
        {
            RunRho("a = 1 + 2");
            //AssertPop(3);
        }
    }
}
