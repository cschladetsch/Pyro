using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestNativeObjects : TestCommon
    {
        [Test]
        public void TestString()
        {
            const string prelude = @"""foobar"" 's # ";

            Run(prelude + "s 'Length .@");
            AssertPop(6);
            Run(prelude + "0 3 s 'Substring .@ &");
            AssertPop("foo");
            Run(prelude + "3 3 s 'Substring .@ &");
            AssertPop("bar");
        }
    }
}