using System.Collections.Generic;
using NUnit.Framework;
using Pyro.Language;

namespace Pyro.Test.Rho
{
    using Exec;

    /// <inheritdoc />
    /// <summary>
    /// Test basic Rho functionality: boolean logic, arithmetic, strings, conditionals.
    /// </summary>
    [TestFixture]
    public class TestRhoBasic
        : TestCommon
    {
        [Test]
        public void TestArrays()
        {
            RhoRun("[1 2 3]", true, EStructure.Expression);
            var list = Pop<List<object>>();
            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0], 1);
            Assert.AreEqual(list[1], 2);
            Assert.AreEqual(list[2], 3);
        }

        //[Test]
        public void TestFloats()
        {
            AssertEqual("1.1 + 2.2", 1.1f + 2.2f);
            AssertEqual("1.1 - 2.2", 1.1f - 2.2f);
            AssertEqual("1.1*2.2", 1.1f * 2.2f);
            AssertEqual("1.1/2.2", 1.1f / 2.2f);
        }

        private void AssertEqual(string rho, object val)
        {
            RhoRun(rho);
            Assert.AreEqual(val, Pop());
        }

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

            RhoRun(@"
                assert(true)
                assert(!false)");
        }

        private void False(string text)
            => Assert.Throws<AssertionFailedException>(() => True(text));

        private void True(string text)
            => RhoRun($"assert({text})");

        [Test]
        public void TestArithmetic()
        {
            RhoRun("b = 1");
            AssertVarEquals("b", 1);

            RhoRun("a = 1 + 2");
            AssertVarEquals("a", 3);
            RhoRun(@"a = 1
                b = 2
                c = (a + b)*2
                writeln(c)
                ");
            AssertVarEquals("c", 6);
        }

        [Test]
        public void TestSequence()
        {
            var code1 = RhoTranslate( @"1").Code;
            Assert.AreEqual(1, code1.Count);
            Assert.AreEqual(1, code1[0]);

            var code2 = RhoTranslate(
                @"
                1
                2
                ").Code;

            Assert.AreEqual(2, code2.Count);
            Assert.AreEqual(1, code2[0]);
            Assert.AreEqual(2, code2[1]);
        }

        [Test]
        public void TestParseCall()
        {
            var prog = RhoTranslate(@"foo()");
            var code = prog.Code;
            Assert.AreEqual(2, code.Count);
            var name = ConstRef<Label>(code[0]);
            var op = ConstRef<EOperation>(code[1]);
            Assert.AreEqual("foo", name.ToString());
            Assert.AreEqual(EOperation.Suspend, op);
        }

        [Test]
        public void TestParseFunDef()
        {
            var code = RhoTranslate(
@"
fun foo()
	1
	2
	3
").Code;
            Assert.AreEqual(3, code.Count);
            Assert.AreSame(typeof(Continuation), code[0].GetType());
            Assert.AreEqual("'foo", code[1].ToString());
            Assert.AreEqual(EOperation.Store, code[2]);

            var cont = ConstRef<Continuation>(code[0]);
            Assert.AreEqual(1, cont.Code[0]);
            Assert.AreEqual(2, cont.Code[1]);
            Assert.AreEqual(3, cont.Code[2]);
        }

        [Test]
        public void TestSimpleFunctions()
        {
            RhoRun("a = 1");
            RhoRun(
@"fun foo()
	1
");

            RhoRun(
@"fun foo()
	1
foo()
");
            AssertPop(1);

            RhoRun(
@"fun foo()
	1
assert(foo() == 1)
");

            RhoRun(
@"fun foo(a)
	a
foo(42)
");

            RhoRun(
@"
fun foo()
	1
fun bar(f, n)
	n + f()
assert(bar(foo, 1) == 2)
assert(bar(foo, 2) == 3)
");

        }

        [Test]
        public void TestConditionals()
        {
            var ifThen =
@"
if true
	1
";
            RhoRun(ifThen);
            AssertPop(1);

            var ifThenElse1 = @"
if true
	1
else
	2
";
            RhoRun(ifThenElse1);
            AssertPop(1);

            var ifThenElse2 = @"
if false
	1
else
	2
";
            RhoRun(ifThenElse2);
            AssertPop(2);
        }

        [Test]
        public void TestWriteln()
        {
            var text = @"writeln(""testing 1 2 3"")";
            RhoRun(text);
        }

        [Test]
        public void TestLocalScope()
        {
            var text = @"
fun foo(a)
	b = a + 1
	assert(b == 3)
foo(2)
";
            RhoRun(text);
        }

        [Test]
        public void TestNestedFunctions()
        {
            RhoRun(@"
fun foo()
	fun bar(f, num)
		1 + f(num)
	fun spam(num)
		num + 2
	bar(spam, 3)
assert(foo() == 6)
");
        }
    }
}

