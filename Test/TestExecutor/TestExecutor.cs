using System.Collections.Generic;
using Diver.Exec;
using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestExecutor : TestCommon
    {
        public void TestPrimitives(object a, object b, object sum)
        {
            var code = _reg.Add(new List<object>());
            var coro = _reg.Add(new Continuation(code.Value));

            code.Value.Add(a);
            code.Value.Add(b);
            code.Value.Add(_reg.Add(EOperation.Plus));

            _exec.Continue(coro);

            var data = _exec.DataStack;
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(sum, data.Pop());
        }

        [Test]
        public void TestIntegers()
        {
            TestPrimitives(1,2,3);
        }

        [Test]
        public void TestStrings()
        {
            TestPrimitives("foo", "bar", "foobar");
        }
    }
}
