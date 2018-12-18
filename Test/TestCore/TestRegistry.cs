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
        public void TestBuiltIns()
        {
            var reg = new Impl.Registry();

            var refNum = reg.AddVal(42);
            Assert.IsNotNull(refNum);
            Assert.IsNotNull(refNum.BaseValue);

            var num = reg.Get(refNum.Id);
            Assert.AreEqual(num, 42);

            var refStr = reg.AddVal("Foo");
            var str = reg.Get(refStr.Id);
            Assert.AreEqual(str, "Foo");
        }
    }
}

