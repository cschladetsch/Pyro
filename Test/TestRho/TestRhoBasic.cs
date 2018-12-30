using Diver.Exec;
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
            True("true");
            True("!false");
            True("!!true");
            True("true == true");
            True("true || false");
            True("true != false");
            True("true ^ false");
            True("!(true && false)");
            True("(true || false) ^ false");
            True("!!(true ^ false)");

            False("false");
            False("!true");
            False("true && false");
            False("true ^ true");
            False("!!(true ^ true)");
            False("false || false");
            False("!true || !true");
            False("!true && !true");
        }

        private void False(string text)
        {
            Assert.Throws<AssertionFailedException>(() => True(text));
        }

        private void True(string text)
        {
            RunRho($"assert({text})", EStructure.Statement);
        }

        [Test]
        public void TestArithmetic()
        {
            RunRho("a = 1 + 2");
            AssertVarEquals("a", 3);
            RunRho(@"
                a = 1
                b = 2
                c = (a + b)*2
                ", EStructure.Program);
            AssertVarEquals("c", 6);
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
@"fun foo()
	1
assert(foo() == 1)
", EStructure.Program);

            RunRho(
@"
fun foo(a)
	a
foo(42)
", EStructure.Program);

            RunRho(
@"
fun foo()
	1
fun bar(f)
	2 + f()
assert(bar(foo) == 3)
assert(bar(foo) == 3)
", EStructure.Program);

        }

        [Test]
        public void TestConditionals()
        {
            var ifThen = @"
if true
	1 + 2
";
            RunRho(ifThen);
            AssertPop(3);

            var ifThenElse1 = @"
if true
	1 - 2
else
	2
";
            RunRho(ifThenElse1);
            AssertPop(-1);

            var ifThenElse2 = @"
if false
	1
else
	2
";
            RunRho(ifThenElse2);
            AssertPop(2);
        }
    }
}
