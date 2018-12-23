using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diver.Language.PiLang;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Diver.Test
{
    [TestFixture]
    public class TestTranslator : TestCommon
    {
        [Test]
        public void Test()
        {
            var text = "1 2 +";
            var trans = new Translator(_reg, text);
            Debug.WriteLine($"Trans.Error= '{trans.Error}");
            Assert.IsFalse(trans.Failed);

            _exec.Continue(trans.Continuation);
            Assert.AreEqual(3, Pop<int>());
        }
    }
}
