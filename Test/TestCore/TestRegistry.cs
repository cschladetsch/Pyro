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
        public void TestMethod1()
        {
            var reg = new Impl.Registry();
            reg.Add(42);
        }
    }
}

