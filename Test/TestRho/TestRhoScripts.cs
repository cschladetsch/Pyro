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
        public void Print()
        {
            _Exec.Scope["pr"] = Function<object>(DebugTrace);
            TestScript("Print.rho");
        }

        [Test]
        public void RunSomeRhoScripts()
        {
            BuiltinTypes.BuiltinTypes.Register(_Registry);

            _Exec.Scope["TimeNow"] = Function(() => DateTime.Now);
            _Exec.Scope["Print"] = Function<TimeSpan>(d => DebugTraceLine(d.ToString()));
            _Exec.Scope["pr"] = Function<object>(DebugTrace);

            TestScript("Functions.rho");
            TestScript("Add.rho");
            TestScript("Arithmetic.rho");
            TestScript("Arithmetic.rho");
            TestScript("Array.rho");
            TestScript("Comments.rho");
            TestScript("Variables.rho");
            TestScript("Strings.rho");
            TestScript("Arithmetic.rho");

            // Failing:
            //TestScript("Coros.rho");
            //TestScript("NestedLoops.rho");
            //TestScript("Yielding.rho");
            //TestScript("ForLoops.rho");
            //TestScript("Conditionals.rho");
            //TestScript("RangeLoops.rho");
            //TestScript("ForLoops.rho");
            //TestScript("NestedFunctions.rho");
            //TestScript("PassingFunctions.rho");

            // needs re-arch
            //TestScript("FreezeThaw.rho");
        }

        private void DebugTrace(object obj)
        {
            var text = "****> " + obj + " <****";
            TestContext.Out.WriteLine(text);
            System.Diagnostics.Trace.WriteLine(text);
            Console.WriteLine(text);
        }

        //[Test]
        public void RunAllRhoScripts()
        {
            foreach (var file in Directory.GetFiles(GetScriptsPath(), "*.rho"))
                Assert.IsTrue(RunScriptPathname(file));
        }
    }
}

