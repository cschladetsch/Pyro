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
            RhoRun("pr(\"hello\")");
            //TestScript("Print.rho");
        }

        [Test]
        public void TestRhoConditionals()
        {
            var r0 = @"
if (1 == 1)
	true
else
	false
";
            var r1 = @"
if (1 != 1)
	true
else
	false
";
            var c0 = RhoTranslate(r0);
            var c1 = RhoTranslate(r1);
            
            DebugTrace(c0.ToText());
            _Exec.Continue(c0);
            AssertPop(true);
            
            DebugTrace(c1.ToText());
            _Exec.Continue(c1);
            AssertPop(false);
        }
        
        [Test]
        public void TestRhoNestedConditionals()
        {
            string Script(int a, int b)
            {
                return $@"
if ({a} != 1)
	if ({b} == 2)
		10
	else
		20
else
	30
";
            }

            var s0 = Script(1, 1);
            var s1 = Script(2, 2);
            var s2 = Script(2, 3);
            var c0 = RhoTranslate(s0);
            var c1 = RhoTranslate(s1);
            var c2 = RhoTranslate(s2);
            
            _Exec.Continue(c0);
            AssertPop(30);    // a = 1, b = 1 => c = 30
            
            _Exec.Continue(c1);    // a = 2, b = 2 => c= 10
            AssertPop(10);
            
            _Exec.Continue(c2);    // a = 2, b = 3 => c= 20
            AssertPop(20);
        }
        
        
        [Test]
        public void RunSomeRhoScripts()
        {
            BuiltinTypes.BuiltinTypes.Register(_Registry);

            _Exec.Scope["TimeNow"] = Function(() => DateTime.Now);
            _Exec.Scope["Print"] = Function<TimeSpan>(d => DebugTraceLine(d.ToString()));
            _Exec.Scope["pr"] = Function<object>(DebugTrace);

            Verbose = true;
            TestScript("Conditionals0.rho");
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

