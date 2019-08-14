namespace Pyro.Test
{
    using NUnit.Framework;

    [TestFixture]
    public class TestRhoFreezeThaw
        : TestCommon
    {
        [Test]
        public void TestFreezeThaw()
        {
            FreezeThaw("Add.rho");
            FreezeThaw("Variables.rho");
            FreezeThaw("Arithmetic.rho");

            // TODO: these fail
            //FreezeThaw("NestedFunctions.rho");
            //FreezeThaw("PassingFunctions.rho");
            //FreezeThaw("Loops.rho");
        }
    }
}
