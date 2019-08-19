namespace Pyro.Test
{
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class TestPiScripts
        : TestCommon
    {
        [Test]
        public void TestSnippets()
        {
            PiRun("1 1 + 2 == assert");
            PiRun("-1 -1 - 0 == assert");
        }

        [Test]
        public void TestAssignment()
        {
            PiRun("1 'a #");
            Assert.AreEqual(1, _Scope["a"]);
            PiRun("2 'a # a a a + +");
            Assert.AreEqual(6, Pop<int>());
        }

        [Test]
        public void RunScripts()
        {
            //TestScript("Floats.pi");
            TestScript("Arithmetic.pi");
            TestScript("Array.pi");
            TestScript("Boolean.pi");
            TestScript("Comments.pi");
            TestScript("Common.pi");
            TestScript("Conditionals.pi");
            //TestScript("Loops.pi");
            //TestScript("Map.pi");
            TestScript("Relational.pi");
            TestScript("StackOperations.pi");
            TestScript("Strings.pi");
            //TestScript("TestAppCalls.pi");
            //TestScript("TreeScope.pi");
            TestScript("Variables.pi");
            TestScript("Continuations.pi");
        }

        //[Test]
        public void RunAllScripts()
        {
            foreach (var file in Directory.GetFiles(GetScriptsPath(), "*.pi"))
                Assert.IsTrue(RunScriptPathname(file));
        }
    }
}
