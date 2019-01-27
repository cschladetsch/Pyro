using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiFreezeThaw : TestCommon
    {
        [SetUp]
        public new void Setup()
        {
            _exec.Rethrows = true;
        }

        [Test]
        public void TestFreezeThaw()
        {
            TestFreezeThawPi("true assert");
            TestFreezeThawScript("Boolean.pi");

            TestFreezeThawPi("1 2 +");
            AssertPop(3);

            TestFreezeThawPi("{1 2 +} &");
            AssertPop(3);

            TestFreezeThawPi(@"""foo"" ""bar"" +");
            AssertPop("foobar");

            TestFreezeThawPi("[1 2 3]");
            var list = Pop<List<object>>();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);

            TestFreezeThawScript("Comments.pi");
            TestFreezeThawScript("Arithmetic.pi");
            TestFreezeThawScript("Array.pi");
            TestFreezeThawScript("Conditionals.pi");
            TestFreezeThawScript("Continuations.pi");
            TestFreezeThawScript("Strings.pi");
        }
    }
}