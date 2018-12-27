using System.Collections.Generic;
using System.Linq;
using Diver.Exec;
using NUnit.Framework;
using Diver.Language;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiTranslator : TestCommon
    {
        [Test]
        public void TestAdd()
        {
            Run("1 2 +");
            Assert.AreEqual(3, Pop<int>());
            Assert.AreEqual(0, DataStack.Count);
        }

        [Test]
        public void TestEquiv()
        {
            Run("1 1 ==");
            Assert.AreEqual(true, Pop<bool>());
            Assert.AreEqual(0, DataStack.Count);
            Run("1 2 ==");
            Assert.AreEqual(false, Pop<bool>());
            Assert.AreEqual(0, DataStack.Count);
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
        public void TestVars()
        {
            Run("1 'a # a 2 +");
            Assert.IsTrue(_scope.ContainsKey("a"));
            var a = _scope["a"];
            Assert.AreEqual(1, a);
            Assert.AreEqual(1, DataStack.Count);
            Assert.AreEqual(1 + 2, _exec.Pop<int>());
            Assert.AreEqual(0, DataStack.Count);
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

        private void Run(string text)
        {
            _exec.Clear();
            _exec.Continue(Translate(text));
        }

        private void BreakRun(string text)
        {
            Assert.Throws<DebugBreakException>(() => Run(text));
        }

        private IRef<Continuation> Translate(string text)
        {
            var trans = new PiTranslator(_reg, text);
            WriteLine(trans.ToString());
            if (trans.Failed)
                WriteLine($"Translation error: {trans.Error}");
            Assert.IsFalse(trans.Failed);
            return _continuation = trans.Continuation;
        }

        private IRef<Continuation> _continuation;
        private Dictionary<string, object> _scope => _continuation.Value.Scope;
        private IList<object> _code => _continuation.Value.Code;
    }
}
