﻿using Diver.Exec;
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

            RunRho(@"
                assert(true)
                assert(!false)");
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
            RunRho("a = 1 + 2", EStructure.Statement);
            AssertVarEquals("a", 3);
            RunRho(@"a = 1
                b = 2
                c = (a + b)*2
                writeln(c)
                ", EStructure.Program);
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
        public void TestExecution()
        {
            RunRho(
@"fun foo()
	1
");
            
            RunRho(
@"fun foo()
	1
foo()
");
            AssertPop(1);

            RunRho(
@"fun foo()
	1
assert(foo() == 1)
");

            RunRho(
@"fun foo(a)
	a
foo(42)
");

            RunRho(
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
            RunRho(ifThen);
            AssertPop(1);

            var ifThenElse1 = @"
if true
	1
else
	2
";
            RunRho(ifThenElse1);
            AssertPop(1);

            var ifThenElse2 = @"
if false
	1
else
	2
";
            RunRho(ifThenElse2);
            AssertPop(2);
        }

        [Test]
        public void TestWriteln()
        {
            var text = @"writeln(""testing 1 2 3"")";
            RunRho(text);
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
            RunRho(text);
        }

        [Test]
        public void TestNestedFunctions()
        {
            RunRho(@"
fun bar(f, num)
	        1 + f(num)
        fun spam(num)
	        num + 2
        bar(spam, 3)
foo()
");
            AssertPop(6);
        }
    }
}
