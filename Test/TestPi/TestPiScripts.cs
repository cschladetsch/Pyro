namespace Pyro.Test
{
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class TestPiScripts
        : TestCommon
    {
        [Test]
        public void RunScript()
        {
            //_exec.TraceLevel = 100
            TestScript("ResumeAfter.pi");
            //TestScript("StackOperations.pi");
            //TestScript("Relational.pi");
            //TestScript("Comments.pi");
            //TestScript("Arithmetic.pi");

            //TestScript("Boolean.pi");
            //TestScript("Array.pi");
            //TestScript("Conditionals.pi");
            //TestScript("Continuations.pi");
            //TestScript("Strings.pi");
        }

        //[Test]
        public void RunAllScripts()
        {
            foreach (var file in Directory.GetFiles(GetScriptsPath(), "*.pi"))
                Assert.IsTrue(RunScriptPathname(file));
        }
    }
}
