using System;
using System.IO;
using System.Threading;
using Diver.Language;
using NUnit.Compatibility;
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
            //Assert.IsTrue(RunScript("Comments.pi"));
            //Assert.IsTrue(RunScript("Arithmetic.pi"));
            Assert.IsTrue(RunScript("Boolean.pi"));
        }

        [Test]
        public void RunAllScripts()
        {
            foreach (var file in Directory.GetFiles(GetScriptsPath(), "*.pi"))
                Assert.IsTrue(RunScriptPathname(file));
        }

    }
}