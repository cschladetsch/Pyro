using System.IO;
using NUnit.Framework;

namespace Diver.Test.Rho
{
    [TestFixture]
    public class TestRhoScripts : TestCommon
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