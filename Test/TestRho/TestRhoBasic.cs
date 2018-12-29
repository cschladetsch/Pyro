using System.Diagnostics.Contracts;
using Diver.Language;
using NUnit.Framework;

namespace Diver.Test.Rho
{
    [TestFixture]
    public class TestRhoBasic : TestCommon
    {
        [Test]
        public void TestBoolean()
        {
            Ensure("true");
            Ensure("!false");
            Ensure("!!true");
            Ensure("true == true");
            Ensure("true || false");
            Ensure("true != false");
            Ensure("true ^ false");
            Ensure("!(true && false)");
            Ensure("(true || false) ^ false");
            Ensure("!!(true ^ false)");

            Fail("false");
            Fail("!true");
            Fail("true && false");
            Fail("true ^ true");
            Fail("!!(true ^ true)");
            Fail("false || false");
            Fail("!true || !true");
            Fail("!true && !true");
        }

        private void Fail(string text)
        {
            Assert.Throws<AssertionFailedException>(() => Ensure(text));
        }

        private void Ensure(string text)
        {
            RunRho($"assert({text})");
        }

        [Test]
        public void TestArithmetic()
        {
            RunRho(
@"
a = 1
b = 2
c = (a + b)*2
", EStructure.Program);
            AssertVarEquals("c", 6);
            RunRho("a = 1 + 2");
            AssertVarEquals("a", 3);
        }

        private void AssertVarEquals<T>(string ident, T val)
        {
            Assert.IsTrue(_scope.ContainsKey(ident));
            var obj = _scope[ident];
            switch (obj)
            {
                case T v:
                    Assert.AreEqual(v, val);
                    return;
                case IRefBase rb:
                    Assert.AreEqual(rb.BaseValue, val);
                    return;
            }
        }

        [Test]
        public void TestFunction()
        {
            RunRho(
@"
fun foo()
    1
assert(foo() == 1)
", EStructure.Function);
        }
    }
}
