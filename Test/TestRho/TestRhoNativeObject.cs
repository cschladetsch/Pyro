namespace Pyro.Test.Rho
{
    using NUnit.Framework;

    public class Inner
    {
        public string s;
    }

    public class Class
    {
        public int n;
        public Inner inner = new Inner();
    }

    [TestFixture]
    public class TestRhoNativeObject
        : TestCommon
    {
        [Test]
        public void TestString()
        {
            RhoRun(
@"a = ""foobar""
");
            AssertVarEquals("a", "foobar");

            RhoRun(
@"a = ""foobar""
b = a.Substring(0, 3)
");
            AssertVarEquals("b", "foo");

            RhoRun(
@"a = ""foobar""
//assert(a.Length == 6)
b = a.Substring(0, 3).Substring(0,1)
//assert(b == ""f"")
//assert(b.Length == 1)
//assert(""foobar"".Substring(0,2).Length == 2)
");

            RhoRun(
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

        [Test]
        public void TestNew()
        {
            RhoRun(@"
k=new ""Pyro.Test.Rho.Class,TestRho"" // Need to use fully-qualified name and also Assembly name.
k.n=42
k.inner.s=""foo""
k
");
            var k = Pop<Class>();
            Assert.AreEqual(typeof(Class), k.GetType());
            Assert.AreEqual(42, k.n);
            Assert.AreEqual("foo", k.inner.s);
        }
    }
}

