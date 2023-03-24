namespace Pyro.Test {
    using Exec;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public class TestExecutor
        : TestCommon {
        [Test]
        public void TestOne() {
            PiRun("42");
            AssertPop<int>(42);
        }

        [Test]
        public void TestAddIntegers() {
            TestAdd(1, 2, 3);
        }

        [Test]
        public void TestAddFloats() {
            TestAdd(1.1f, 1.1f, 1.1f + 1.1f);
        }

        [Test]
        public void TestAddStrings() {
            TestAdd("foo", "bar", "foobar");
        }

        private void TestAdd(object a, object b, object sum) {
            var code = _Registry.Add(new List<object>());
            var coro = _Registry.Add(new Continuation(code.Value));

            code.Value.Add(a);
            code.Value.Add(b);
            code.Value.Add(EOperation.Plus);

            _Exec.Continue(coro);

            var data = _Exec.DataStack;
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(sum, data.Pop());
        }

        // Doesn't work due to changes in how execution works
        // [Test]
        // public void TestExecSingleStep1() {
        //     var cont = PiTranslate("1 2");
        //     _Exec.Continue(cont);
        //     _Exec.Next();
        //     AssertPop(1);
        //     _Exec.Next();
        //     AssertPop(2);
        //     _Exec.Next();
        //     Assert.AreEqual(0, _Exec.DataStack.Count);
        // }
    }
}

