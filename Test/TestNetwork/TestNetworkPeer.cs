using Diver.Network;
using Diver.Test;
using NUnit.Framework;
using Pyro.ExecutionContext;

namespace Pyro.Test
{
    [TestFixture]
    public class TestNetworkPeer : TestCommon
    {
        public const int ListenPort = 8888;
        private Context _context;

        [SetUp]
        public new void Setup()
        {
            base.Setup();
            _context = new Context();
        }

        [Test]
        public void TestSelfHosting()
        {
            var peer = Create.NewPeer(ListenPort);
            peer.OnConnected += Connected;
            peer.OnReceivedResponse += Received;

            Assert.IsTrue(peer.Start(), peer.Error);
            //Assert.IsTrue(peer.Execute("pi"), peer.Error);

            Assert.IsTrue(peer.Execute("true assert"), peer.Error);
            Assert.IsTrue(peer.Execute("1 2 +"), peer.Error);
        }

        private void Connected(IPeer peer, IClient client)
        {
            WriteLine($"Connected to {client}");
        }

        private void Received(IServer server, IClient client, string response)
        {
            Assert.IsNotEmpty(response);
            //_context.Language = ELanguage.Pi;
            //Assert.IsTrue(_context.Exec(response));
            //var stack = _context.Executor.DataStack;
            //Assert.AreEqual(1, stack.Count);
            //Assert.AreEqual(3, stack.Pop());
        }
    }
}
