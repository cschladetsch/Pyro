using System;
using NUnit.Framework;

namespace Diver.TestCore
{
    public static class Static
    {
        public static int Foo;
    }

    [TestFixture]
    public class TestRegistry
    {
        [Test]
        public void TestBuiltins()
        {
            var reg = new Impl.Registry();

            var refNum = reg.Add(42);
            var num = reg.Get<int>(refNum.Id);
            Assert.AreEqual(num, 42);

            var refStr = reg.Add("Foo");
            var str = reg.Get<string>(refStr.Id);
            Assert.AreEqual(str, "Foo");
        }
    }
}

