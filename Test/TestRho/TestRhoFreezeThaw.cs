using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestRhoFreezeThaw : TestCommon
    {
        [Test]
        public void TestFreezeThaw()
        {
            TestFreezeThawScript("Loops.rho");
            TestFreezeThawScript("NestedFunctions.rho");
            TestFreezeThawScript("PassingFunctions.rho");
            TestFreezeThawScript("Variables.rho");
            TestFreezeThawScript("Arithmetic.rho");
        }
    }
}