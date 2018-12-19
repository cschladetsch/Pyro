using Diver.Core.Network;
using Diver.Network;
using NUnit.Core;
using NUnit.Framework;

namespace Diver.Tests
{
    [TestFixture]
    public class TestPeer
    {
        [Test]
        public void TestLocalPeer()
        {
            var reg = new Impl.Registry();

            IRef<Peer> peer = reg.New<Peer>();
        }
    }
}
