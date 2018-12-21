using System;
using System.Diagnostics;
using NUnit.Framework;

using Diver.Language;
using Diver.Language.PiLang;

namespace Diver.Test
{
    [TestFixture()]
    class TestPiParser : TestCommon
    {
        [Test]
        public void TestSimpleTokens()
        {
            var lexer = new Lexer("1 2 3 + +");
            Assert.IsTrue(lexer.Process());
            var parser = new Parser(lexer);
            if (parser.Failed)
                Debug.WriteLine(parser.Error);
            Assert.IsTrue(parser.Process(lexer, EStructure.None));

            Debug.WriteLine(parser.PrintTree());
            Console.WriteLine(parser.PrintTree());

            //var root = parser.Root;
            //var code = root.Value;
            //var cont = _reg.Add(new Continuation(code as List<object>));
            //_exec.Continue(cont);
        }
    }
}
