﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diver.Exec;
using Diver.Language.PiLang;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Diver.Test
{
    [TestFixture]
    public class TestTranslator : TestCommon
    {
        [Test]
        public void TestAdd()
        {
            Run("1 2 +");
            Assert.AreEqual(3, Pop<int>());
        }

        [Test]
        public void TestVars()
        {
            Run("42 a #");
            Assert.AreSame(0, DataStack.Count);
            Assert.IsTrue(_scope.ContainsKey("a"));
            var a = _scope["a"];
            Assert.AreSame(42, a);
        }

        [Test]
        public void TestAddString()
        {
            Run("\"foo\" \"bar\" +");
            Assert.AreEqual("barfoo", Pop<string>());
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
            _exec.Continue(Translate(text));
        }

        private Continuation Translate(string text)
        {
            var trans = new Translator(_reg, text);
            Debug.WriteLine($"Trans.Error= '{trans.Error}");
            Assert.IsFalse(trans.Failed);
            return _continuation = trans.Continuation.Value;
        }

        private Continuation _continuation;
        private Dictionary<string, object> _scope => _continuation.Scope;
        private IList<object> _code => _continuation.Code;
    }
}
