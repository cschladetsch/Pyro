using System.Collections.Generic;
using Diver.Exec;
using NUnit.Framework;

namespace Diver.Tests
{
    [TestFixture]
    public class TestExecutor : TestCommon
    {
        [Test]
        public void TestPrimitives()
        {
            var code = _reg.Add(new List<object>());
            var coro = _reg.Add(new Continuation(code.Value));

            code.Value.Add(1);
            code.Value.Add(2);
            code.Value.Add(_reg.Add(EOperation.Plus));

            _exec.Continue(coro);

            var data = _exec.DataStack;
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(3, data.Pop());
        }
    }
}
