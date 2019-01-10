using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestRhoFreezeThaw : TestCommon
    {
        [Test]
        public void TestFreezeThaw()
        {
            TestFreezeThawScript("Add.rho");
            TestFreezeThawScript("Variables.rho");
            TestFreezeThawScript("Arithmetic.rho");

            // TODO: these fail
            //TestFreezeThawScript("NestedFunctions.rho");
            //TestFreezeThawScript("PassingFunctions.rho");
            //TestFreezeThawScript("Loops.rho");
        }
    }
}