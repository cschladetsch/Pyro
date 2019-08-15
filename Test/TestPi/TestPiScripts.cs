namespace Pyro.Test
{
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class TestPiScripts
        : TestCommon
    {
        [Test]
        public void RunScripts()
        {
            TestScript("Arithmetic.pi");
            TestScript("Array.pi");
            TestScript("Boolean.pi");
            TestScript("Comments.pi");
            TestScript("Common.pi");
            TestScript("Conditionals.pi");
            //TestScript("Floats.pi");
            //TestScript("Loops.pi");
            //TestScript("Map.pi");
            TestScript("Relational.pi");
            TestScript("StackOperations.pi");
            TestScript("Strings.pi");
            //TestScript("TestAppCalls.pi");
            //TestScript("TreeScope.pi");
            //TestScript("Variables.pi");
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
