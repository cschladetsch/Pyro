namespace Pyro.Test.Rho
{
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class TestRhoScripts
        : TestCommon
    {
        [Test]
        public void RunSomeScripts()
        {
            TestScript("Loops.rho");
            TestScript("NestedFunctions.rho");
            TestScript("PassingFunctions.rho");
            TestScript("Variables.rho");
            TestScript("Arithmetic.rho");
            TestScript("Array.rho");
            //TestScript("FreezeThaw.rho");
        }

        //[Test]
        public void RunAllScripts()
        {
            foreach (var file in Directory.GetFiles(GetScriptsPath(), "*.rho"))
                Assert.IsTrue(RunScriptPathname(file));
        }
    }
}

