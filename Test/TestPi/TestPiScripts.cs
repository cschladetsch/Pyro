using System.IO;
using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiScripts : TestCommon
    {
        [Test]
        public void RunScript()
        {
            //_exec.TraceLevel = 100;
            //TestScript("Comments.pi");
            //TestScript("Arithmetic.pi");
            //TestScript("Boolean.pi");
            //TestScript("Array.pi");
            TestScript("Conditionals.pi");
        }

        private void TestScript(string scriptName)
        {
            Assert.IsTrue(RunScript(scriptName));
        }

        //[Test]
        public void RunAllScripts()
        {
            foreach (var file in Directory.GetFiles(GetScriptsPath(), "*.pi"))
                Assert.IsTrue(RunScriptPathname(file));
        }
    }
}