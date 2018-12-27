using System.Collections.Generic;
using System.Linq;
using Diver.Exec;
using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiTranslator : TestCommon
    {
        [Test]
        public void TestContinuations()
        {
            Run("1 1 {+} &");
            AssertPop(2);
            AssertEmpty();

            Run("1 2 { + } & 3 ==");
            AssertPop(true);
            AssertEmpty();

            Run("2 3 4 { + * } &");
            AssertPop(14);
            AssertEmpty();

            Run("3 'a # { {1 +} 'a # 3 a &} & a +");
            AssertPop(7);
            AssertEmpty();
        }

        [Test]
        public void TestConditionalExec()
        {
            Run("1 { 1 + } { 2 + } true ife &");
            AssertPop(2);

            Run("1 { 1 + } { 2 + } false ife &");
            AssertPop(3);
        }

        [SetUp]
        public void LoadCommon()
        {
            RunScript("Common.pi");
        }

        [Test]
        public void TestDebugList()
        {
            Run("1 2 3 [3 4 5] \"foo\" debug_datastack");
        }

        [Test]
        public void TestDepth()
        {
            Run("depth");
            Assert.AreEqual(0, Pop<int>());
            Run("1 2 3 depth");
            Assert.AreEqual(3, Pop<int>());
        }

        [Test]
        public void TestIdents()
        {
            Assert.Throws<UnknownIdentifierException>(() => Run("asdasd"));
        }

        [Test]
        public void TestArith()
        {
            Run("1 2 +");
            AssertPop(3);
            AssertEmpty();

            Run("3 -1 +");
            AssertPop(2);
            AssertEmpty();

            Run("6 2 *");
            AssertPop(12);
            AssertEmpty();

            Run("6 2 div");
            AssertPop(3);
            AssertEmpty();
        }

        [Test]
        public void TestEquiv()
        {
            Run("1 1 ==");
            AssertPop(true);
            AssertEmpty();
            Run("1 2 ==");
            AssertPop(false);
            AssertEmpty();
            Run("\"foo\" \"foo\" ==");
            AssertPop(true);
            AssertEmpty();
            Run("\"foo\" \"bar\" ==");
            AssertPop(false);
            AssertEmpty();

            Run("false true and");
            AssertPop(false);
            AssertEmpty();

            Run("false true and not");
            AssertPop(true);
            AssertEmpty();

            Run("false true or");
            AssertPop(true);
            AssertEmpty();

            Run("true false and");
            AssertPop(false);
            AssertEmpty();

            Run("true false and not");
            AssertPop(true);
            AssertEmpty();
        }

        [Test]
        public void TestBreak()
        {
            Assert.Throws<DebugBreakException>(
                () => Run("break"));
        }

        [Test]
        public void TestAssertion()
        {
            Run("true assert");
            Run("true not not assert");
            Assert.Throws<AssertionFailedException>(
                () => Run("false assert"));
        }

        [Test]
        public void TestNegativeInts()
        {
            Run("-1345 -0");
            AssertPop(0);
            AssertPop(-1345);
            AssertEmpty();
        }

        [Test]
        public void TestVars()
        {
            Run("1 'a # a 2 +");
            Assert.IsTrue(_scope.ContainsKey("a"));
            var a = _scope["a"];
            Assert.AreEqual(1, a);
            Assert.AreEqual(1, DataStack.Count);
            Assert.AreEqual(1 + 2, Pop<int>());
            AssertEmpty();
        }

        [Test]
        public void TestAddString()
        {
            Run("\"foo\" \"bar\" +");
            Assert.AreEqual("foobar", Pop<string>());
        }

        [Test]
        public void TestArray()
        {
            Run("[1 2 [3 4 5]]");
            var list = Pop<List<object>>();
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            var inner = list[2] as IList<object>;
            Assert.IsNotNull(inner);
            Assert.IsTrue(inner.SequenceEqual(new object[] {3,4,5}));
        }

        private void BreakRun(string text)
        {
            Assert.Throws<DebugBreakException>(() => Run(text));
        }
    }
}
