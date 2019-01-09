using System;
using System.Collections.Generic;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;
using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiFreezeThaw : TestCommon
    {
        private new IRegistry _reg;
        //private new Executor _exec;
        private ITranslator _pi;
        private ITranslator _rho;

        [SetUp]
        public new void Setup()
        {
            _reg = new Registry();
            _pi = new PiTranslator(_reg);
            _rho = new PiTranslator(_reg);
            //_exec = new Executor(_reg);
            _exec.Rethrows = true;
        }

        [Test]
        public void TestFreezeThaw()
        {
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
            TestFreezeThawScript("Boolean.pi");
            TestFreezeThawScript("Array.pi");
            TestFreezeThawScript("Conditionals.pi");
            TestFreezeThawScript("Continuations.pi");
            TestFreezeThawScript("Strings.pi");
        }

        private void TestFreezeThawScript(string fileName)
        {
            var text = TranslateScript(fileName).ToText();
            TestFreezeThawPi(text);
        }

        protected void TestFreezeThaw(Continuation cont)
        {
            var text = cont.ToText();
            
        }

        protected void TestFreezeThawPi(string text)
        {
            Assert.IsTrue(Continue(FreezeThaw(_pi, text)));            
        }

        protected void TestFreezeThawRho(string text)
        {
            Assert.IsTrue(Continue(FreezeThaw(_rho, text)));            
        }

        protected bool Continue(Continuation cont)
        {
            Assert.IsNotNull(cont);
            try
            {
                _exec.Continue(cont);
                return true;
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
                return false;
            }
        }

        protected Continuation FreezeThaw(ITranslator trans, string text)
        {
            WriteLine("--- Input:");
            WriteLine(text);
            var cont = PiTranslate(text);

            WriteLine("--- Serialised:");
            var str = cont.ToText();
            WriteLine(str);
            Assert.IsNotEmpty(str);

            var thawed = PiTranslate(str);
            Assert.IsNotNull(thawed);
            var continuation = thawed.Code[0] as Continuation;
            Assert.IsNotNull(continuation);
            return continuation;
        }
    }
}