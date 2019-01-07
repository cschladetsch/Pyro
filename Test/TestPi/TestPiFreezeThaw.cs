using Diver.Exec;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiFreezeThaw : TestCommon
    {
        [Test]
        public void TestFreezeThaw()
        {
            //Continue(FreezeThaw())
        }

        protected Continuation FreezeThaw(string text)
        {
            var cont = PiTranslate(text);
            var str = cont.Serialise();
            Assert.IsNotEmpty(str);
            var thawed = PiTranslate(str);
            Assert.IsNotNull(thawed);
            var rb = thawed.Code[0] as IRefBase;
            return rb.BaseValue as Continuation;
        }
    }
}