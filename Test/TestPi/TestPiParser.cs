using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

using Diver.Language;

namespace Diver.Test
{
    [TestFixture()]
    class TestPiParser : TestCommon
    {
        protected internal PiParser PiParser;
        protected internal IList<PiAstNode> Sequence => PiParser.Root.Children;
        protected internal PiAstNode First => Sequence?[0];

        [Test]
        public void TestSimpleTokens()
        {
            var lexer = new PiLexer("1 2 3");
            Assert.IsTrue(lexer.Process());
            var parser = new PiParser(lexer);
            Assert.IsTrue(parser.Process(lexer));
        }

        [Test]
        public void TestPathnames()
        {
            Parse("foo");
            Assert.AreEqual(EPiAst.Ident, First.Type);
            Assert.AreEqual("foo", First.Value.ToString());
            var ident = First.Value as Label;
            Assert.IsNotNull(ident);
            Assert.IsFalse(ident.Quoted);

            Parse("'foo", true);
            Assert.AreEqual(EPiAst.Ident, First.Type);
            var ident1 = First.Value as Label;
            Assert.IsNotNull(ident1);
            Assert.IsTrue(ident1.Quoted);

            Assert.AreEqual("'foo", First.Value.ToString());

            Parse("foo/bar");
            Assert.AreEqual(EPiAst.Pathname, First.Type);
            Assert.AreEqual("foo/bar", First.Value.ToString());

            Parse("'foo/bar");
            Assert.AreEqual(EPiAst.Pathname, First.Type);
            Assert.AreEqual("'foo/bar", First.Value.ToString());

            Parse("'foo/bar/spam");
            Assert.AreEqual(EPiAst.Pathname, First.Type);
            Assert.AreEqual("'foo/bar/spam", First.Value.ToString());
        }

        [Test]
        public void TestArray1()
        {
            Parse("[42 23]");
            var array = Sequence[0];
            Assert.IsNotNull(array);
            var contents = Sequence[0].Children;
            Assert.AreEqual(42, contents[0].Value);
            Assert.AreEqual(23, contents[1].Value);
        }

        private void Parse(string text, bool verbose = false)
        {
            var lexer = new PiLexer(text);
            Assert.IsTrue(lexer.Process());
            if (verbose)
                WriteLine(lexer.ToString());
            if (lexer.Failed)
                WriteLine(lexer.Error);
            PiParser = new PiParser(lexer);
            PiParser.Process(lexer, EStructure.None);
            if (verbose)
                WriteLine(PiParser.PrintTree());
            if (PiParser.Failed)
                Debug.WriteLine(PiParser.Error);
            Assert.IsFalse(PiParser.Failed);
            Assert.IsNotNull(PiParser.Root);
        }
    }
}
