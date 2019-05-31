using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pyro;
using Pyro.Exec;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiTranslator : TestCommon
    {
        [Test]
        public void TestContinuations()
        {
            PiRun("1 1 {+} &");
            AssertPop(2);
            AssertEmpty();

            PiRun("1 2 { + } & 3 ==");
            AssertPop(true);
            AssertEmpty();

            PiRun("2 3 4 { + * } &");
            AssertPop(14);
            AssertEmpty();

            PiRun("3 'a # { {1 +} 'a # 3 a &} & a +");
            AssertPop(7);
            AssertEmpty();
        }

        [Test]
        public void TestConditionalExec()
        {
            PiRun("1 { 1 + } { 2 + } true ife &");
            AssertPop(2);

            PiRun("1 { 1 + } { 2 + } false ife &");
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
            PiRun("1 2 3 [3 4 5] \"foo\" debug_datastack");
        }

        [Test]
        public void TestDepth()
        {
            PiRun("depth");
            Assert.AreEqual(0, Pop<int>());
            PiRun("1 2 3 depth");
            Assert.AreEqual(3, Pop<int>());
        }

        [Test]
        public void TestIdents()
        {
            Assert.Throws<UnknownIdentifierException>(() => PiRun("asdasd"));
        }

        [Test]
        public void TestArith()
        {
            PiRun("1 2 +");
            AssertPop(3);
            AssertEmpty();

            PiRun("3 -1 +");
            AssertPop(2);
            AssertEmpty();

            PiRun("6 2 *");
            AssertPop(12);
            AssertEmpty();

            PiRun("6 2 div");
            AssertPop(3);
            AssertEmpty();
        }

        [Test]
        public void TestEquiv()
        {
            PiRun("1 1 ==");
            AssertPop(true);
            AssertEmpty();
            PiRun("1 2 ==");
            AssertPop(false);
            AssertEmpty();
            PiRun("\"foo\" \"foo\" ==");
            AssertPop(true);
            AssertEmpty();
            PiRun("\"foo\" \"bar\" ==");
            AssertPop(false);
            AssertEmpty();

            PiRun("false true and");
            AssertPop(false);
            AssertEmpty();

            PiRun("false true and not");
            AssertPop(true);
            AssertEmpty();

            PiRun("false true or");
            AssertPop(true);
            AssertEmpty();

            PiRun("true false and");
            AssertPop(false);
            AssertEmpty();

            PiRun("true false and not");
            AssertPop(true);
            AssertEmpty();
        }

        [Test]
        public void TestBreak()
        {
            Assert.Throws<DebugBreakException>(
                () => PiRun("break"));
        }

        [Test]
        public void TestAssertion()
        {
            PiRun("true assert");
            PiRun("true not not assert");
            Assert.Throws<AssertionFailedException>(
                () => PiRun("false assert"));
        }

        [Test]
        public void TestNegativeInts()
        {
            PiRun("-1345 -0");
            AssertPop(0);
            AssertPop(-1345);
            AssertEmpty();
        }

        [Test]
        public void TestVars()
        {
            PiRun("1 'a # a 2 +");
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
            PiRun("\"foo\" \"bar\" +");
            Assert.AreEqual("foobar", Pop<string>());
        }

        [Test]
        public void TestArray()
        {
            PiRun("[1 2 [3 4 5]]");
            var list = Pop<List<object>>();
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            var inner = list[2] as IList<object>;
            Assert.IsNotNull(inner);
            Assert.IsTrue(inner.SequenceEqual(new object[] {3,4,5}));
        }

        private void BreakRun(string text)
        {
            Assert.Throws<DebugBreakException>(() => PiRun(text));
        }
    }
}
