﻿using Diver.Exec;
using NUnit.Framework;

using Diver.LanguageCommon;
using Diver.Tests;

namespace Diver.Test
{
    [TestFixture()]
    class TestPiParser : TestCommon
    {
        [Test]
        public void TestSimpleTokens()
        {
            var lexer = new Diver.PiLang.Lexer("1 2 3 + +");
            Assert.IsTrue(lexer.Process());
            var parser = new Diver.PiLang.Parser(lexer);
            parser.Process(lexer, EStructure.None);

            //var root = parser.Root;
            //var val = root.Value;
            //var cont = _reg.Add(root.Value as Continuation);
            //_exec.Continue(cont);
        }
    }
}
