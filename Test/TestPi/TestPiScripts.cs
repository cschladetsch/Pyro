namespace Pyro.Test {
    using NUnit.Framework;
    using System.IO;

    [TestFixture]
    public class TestPiScripts
        : TestCommon {
        [Test]
        public void TestSnippets() {
            PiRun("1 1 + 2 == assert");
            PiRun("-1 -1 - 0 == assert");
        }

        [Test]
        public void TestAssignment() {
            PiRun("1 'a #");
            Assert.AreEqual(1, _Scope["a"]);
            PiRun("2 'a # a a a + +");
            Assert.AreEqual(6, Pop<int>());
        }

        [Test]
        public void TestExistsAssert() {
            PiRun("'foo exists not assert");
        }

        [Test]
        public void RunSomePiScripts() {
            TestScript("Variables.pi");
            TestScript("Common.pi");
            TestScript("Arithmetic.pi");
            TestScript("Array.pi");
            TestScript("Boolean.pi");
            TestScript("Comments.pi");
            TestScript("Relational.pi");
            TestScript("StackOperations.pi");
            TestScript("Strings.pi");
            TestScript("Continuations.pi");

            //TestScript("Current.pi");
            //TestScript("Floats.pi");
            //TestScript("Conditionals.pi");
            //TestScript("Loops.pi");
            //TestScript("Map.pi");
            //TestScript("TestAppCalls.pi");
            //TestScript("TreeScope.pi");
        }

        [Test]
        public void RunAllPiScripts() {
            foreach (var file in Directory.GetFiles(GetScriptsPath(), "*.pi"))
                Assert.IsTrue(RunScriptPathname(file));
        }
    }
}
