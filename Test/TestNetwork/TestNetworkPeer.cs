using System;
using System.Net;
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
        private Pyro.ExecutionContext.Context _context;

        [SetUp]
        public void Setup()
        {
            base.Setup();
            _context = new Context();
        }

        [Test]
        public void TestSelfHosting()
        {
            var peer = new Peer(ListenPort);
            peer.RecievedResponse += Received;

            Assert.IsTrue(peer.Start());
            Assert.IsTrue(peer.Execute("pi"));

            Assert.IsTrue(peer.Execute("true assert"));
            Assert.IsTrue(peer.Execute("1 2 +"));

            Assert.IsTrue(peer.SelfHost(ListenPort));
        }

        private void Received(IServer server, IClient client, string response)
        {
            Assert.IsNotEmpty(response);
            _context.Language = ELanguage.Pi;
            Assert.IsTrue(_context.Exec(response));
            var stack = _context.Executor.DataStack;
            Assert.AreEqual(1, stack.Count);
            Assert.AreEqual(3, stack.Pop());
        }
    }
}
