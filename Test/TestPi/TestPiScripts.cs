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
            Assert.IsTrue(RunScript("Comments.pi"));
            Assert.IsTrue(RunScript("Arithmetic.pi"));
            Assert.IsTrue(RunScript("Boolean.pi"));
            Assert.IsTrue(RunScript("Array.pi"));
        }

        //[Test]
        public void RunAllScripts()
        {
            foreach (var file in Directory.GetFiles(GetScriptsPath(), "*.pi"))
                Assert.IsTrue(RunScriptPathname(file));
        }
    }
}