namespace Pyro.Test
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Exec;

    [TestFixture]
    public class TestExecutor
        : TestCommon
    {
        [Test]
        public void TestOne()
        {
            PiRun("42");
            AssertPop<int>(42);
        }
        
        [Test]
        public void TestAddIntegers()
        {
            TestAdd(1,2,3);
        }

        [Test]
        public void TestAddFloats()
        {
            TestAdd(1.1f, 1.1f, 1.1f + 1.1f);
        }

        [Test]
        public void TestAddStrings()
        {
            TestAdd("foo", "bar", "foobar");
        }

        private void TestAdd(object a, object b, object sum)
        {
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

        [Test]
        public void TestExecSingleStep1()
        {
            var cont = PiTranslate("1 2");
            _Exec.ContextStack.Add(cont);
            _Exec.Single();
            AssertPop(1);
            _Exec.Single();
            AssertPop(2);
            _Exec.Single();
            Assert.AreEqual(0, _Exec.DataStack.Count);
        }
    }
}

