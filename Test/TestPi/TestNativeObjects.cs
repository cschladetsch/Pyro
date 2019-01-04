using System.Linq;
using Diver.Language;
using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestNativeObjects : TestCommon
    {
        [Test]
        public void TestString()
        {
            AssertSameTokens("#", EPiToken.Store);

            const string pre0 = "\"foobar\" 's # ";
            const string length = pre0 + "'Length s .@";
            const string sub0 = pre0 + "3 0 'Substring s .@ &";
            const string sub1 = pre0 + "3 3 'Substring s .@ &";
            const string sub2 = pre0 + "4 'Substring1 s .@ &";

            AssertSameTokens("s", EPiToken.Ident);
            AssertSameTokens(pre0, EPiToken.String, EPiToken.Quote, EPiToken.Ident, EPiToken.Store);
            AssertSameTokens(sub0, EPiToken.String, EPiToken.Quote, EPiToken.Ident, EPiToken.Store, EPiToken.Int,
                EPiToken.Int, EPiToken.Quote, EPiToken.Ident, EPiToken.Ident, EPiToken.GetMember, EPiToken.Suspend);

            Run(length);
            AssertPop(6);
            Run(sub0);
            AssertPop("foo");
            Run(sub1);
            AssertPop("bar");
            Run(sub2);
            AssertPop("ar");
        }
    }
}