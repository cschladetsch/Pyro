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
            var s0 = PiTranslate("drop").Serialise();
            var cont = PiTranslate("1 2 + [ 2 3 - writeln ] expand drop");
            var str = cont.Serialise();
            WriteLine(str);
        }
    }
}