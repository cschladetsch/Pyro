using System;

namespace Pyro.Test.Rho
{
    using System.IO;
    using NUnit.Framework;
    using static Create;

    [TestFixture]
    public class TestRhoScripts
        : TestCommon
    {
        [Test]
        public void RunSomeScripts()
        {
            BuiltinTypes.BuiltinTypes.Register(_Registry);

            _Exec.Scope["TimeNow"] = Function(() => DateTime.Now);
            _Exec.Scope["Print"] = Function<TimeSpan>(d => DebugTraceLine(d.ToString()));

            TestScript("ForLoops.rho");
            //TestScript("Functions.rho");
            ////TestScript("Coros.rho");
            //TestScript("Add.rho");
            //TestScript("Arithmetic.rho");
            //TestScript("Arithmetic.rho");
            //TestScript("Array.rho");
            //TestScript("Comments.rho");
            //TestScript("Variables.rho");
            //TestScript("Strings.rho");
            //TestScript("Conditionals.rho");
            //TestScript("Arithmetic.rho");
            //TestScript("RangeLoops.rho");
            //TestScript("ForLoops.rho");
            //TestScript("NestedFunctions.rho");
            //TestScript("PassingFunctions.rho");

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

