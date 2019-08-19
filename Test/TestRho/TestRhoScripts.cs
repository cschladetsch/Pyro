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
            BuiltinTypes.BuiltinTypes.Register(_Registry);

            TestScript("ForLoops.rho");
            TestScript("Add.rho");
            TestScript("Arithmetic.rho");
            TestScript("Array.rho");
            TestScript("Comments.rho");
            TestScript("Variables.rho");
            TestScript("Strings.rho");
            TestScript("Conditionals.rho");
            TestScript("Arithmetic.rho");
            TestScript("RangeLoops.rho");
            TestScript("ForLoops.rho");
            TestScript("NestedFunctions.rho");
            TestScript("PassingFunctions.rho");

            // needs re-arch
            //TestScript("NestedLoops.rho");

            // needs re-arch
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

