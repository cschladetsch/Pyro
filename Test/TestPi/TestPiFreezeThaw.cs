using System;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;
using Diver.Language.Impl;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiFreezeThaw : TestCommon
    {
        private new IRegistry _reg;
        private new Executor _exec;
        private ITranslator _pi;
        private ITranslator _rho;

        [SetUp]
        public new void Setup()
        {
            _reg = new Registry();
            _pi = new PiTranslator(_reg);
            _rho = new PiTranslator(_reg);
            _exec = new Executor(_reg);
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

            TestFreezeThawPi(@"""foo"" ""bar"" +");
            AssertPop("foobar");

            TestFreezeThawPi(@"""foo"" ""bar"" +");
            AssertPop("foobar");
        }

        protected void TestFreezeThawPi(string text)
        {
            Assert.IsTrue(Continue(FreezeThaw(_pi, text)));            
        }

        protected void TestFreezeThawRho(string text)
        {
            throw new NotImplementedException();
        }

        protected bool Continue(Continuation cont)
        {
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
            var cont = PiTranslate(text);
            var str = cont.Serialise();
            Assert.IsNotEmpty(str);
            var thawed = PiTranslate(str);
            Assert.IsNotNull(thawed);
            var rb = thawed.Code[0] as IRefBase;
            Assert.IsNotNull(rb);
            return rb.BaseValue as Continuation;
        }
    }
}