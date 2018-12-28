using NUnit.Framework;

namespace Diver.Test.Rho
{
    [TestFixture]
    public class TestRhoBasic : TestCommon
    {
        [Test]
        public void TestBasic()
        {
            RunRho("a = 1");
        }
    }
}
