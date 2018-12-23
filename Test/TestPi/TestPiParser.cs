using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Diver.Exec;
using NUnit.Framework;

using Diver.Language;
using Diver.Language.PiLang;

namespace Diver.Test
{
    [TestFixture()]
    class TestPiParser : TestCommon
    {
        protected internal Parser Parser;
        protected internal IList<AstNode> Sequence => Parser.Root.Children;

        [Test]
        public void TestSimpleTokens()
        {
            var lexer = new Lexer("1 2 3");
            Assert.IsTrue(lexer.Process());
            var parser = new Parser(lexer);
            Assert.IsTrue(parser.Process(lexer));
        }

        [Test]
        public void TestArray1()
        {
            Parse("[42 23]");
            var array = Sequence[0];
            Assert.IsNotNull(array);
            //Assert.AreSame(EAst.Array, array.Type); WTF
            var contents = Sequence[0].Children;
            Assert.AreEqual(42, contents[0].Value);
            Assert.AreEqual(23, contents[1].Value);
        }

        //[Test]
        //public void TestArray2()
        //{
        //    Parse("[\"foo\" [1 2 3]]");
        //    Assert.IsNotNull(Sequence[0]);
        //    var list = Sequence[0] as IList<object>;
        //    Assert.IsNotNull(list);
        //    var nested = Sequence[1] as IList<object>;
        //    Assert.IsNotNull(nested);
        //    Assert.AreEqual("foo", list[0]);
        //    Assert.AreEqual(3, nested.Count);
        //    Assert.AreEqual(1, nested[0]);
        //    Assert.AreEqual(2, nested[1]);
        //    Assert.AreEqual(3, nested[2]);
        //}

        private void Parse(string text)
        {
            var lexer = new Lexer(text);
            Assert.IsTrue(lexer.Process());
            if (lexer.Failed)
                Debug.WriteLine(lexer.Error);
            Parser = new Parser(lexer);
            Parser.Process(lexer, EStructure.None);
            if (Parser.Failed)
                Debug.WriteLine(Parser.Error);
            Assert.IsFalse(Parser.Failed);
            Assert.IsNotNull(Parser.Root);
        }
    }
}
