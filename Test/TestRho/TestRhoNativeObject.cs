﻿using NUnit.Framework;

namespace Diver.Test.Rho
{
    [TestFixture]
    public class TestRhoNativeObject : TestCommon
    {
        [Test]
        public void TestString()
        {
            RunRho(
@"a = ""foobar""
");
            AssertVarEquals("a", "foobar");

            RunRho(
@"a = ""foobar""
b = a.Substring(0, 3)
");
            AssertVarEquals("b", "foo");

            RunRho(
@"a = ""foobar""
assert(a.Length == 6)
b = a.Substring(0, 3).Substring(0,1)
assert(b == ""f"")
assert(b.Length == 1)
//assert(""foobar"".Substring(0,2).Length == 2)
");

            RunRho(
@"a = ""foobar""
b = a.Substring(0, 3)
c = a.Substring(3, 3)
d = a.Substring1(4)
assert(a == ""foobar"")
assert(b == ""foo"")
assert(c == ""bar"")
assert(d == ""ar"")
");
        }
    }
}