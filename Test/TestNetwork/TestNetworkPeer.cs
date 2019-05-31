using System;
using System.Collections;
using Diver.Test;
using Flow;
using NUnit.Framework;
using Pyro.ExecutionContext;
using Pyro.Language;
using Pyro.Network;

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
            var peer = Network.Create.NewPeer(ListenPort);
            peer.OnConnected += Connected;
            peer.OnReceivedResponse += Received;

            Assert.IsTrue(peer.StartSelfHosting(), peer.Error);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            //Assert.IsTrue(peer.Execute("pi"), peer.Error);

            //Assert.IsTrue(peer.Execute("true assert"), peer.Error);
            Assert.IsTrue(peer.Execute("1 2 +"), peer.Error);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            peer.Stop();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        private void Connected(IPeer peer, IClient client)
        {
            WriteLine($"Connected to {client}");
        }

        private void Received(IServer server, IClient client, string response)
        {
            Assert.IsNotEmpty(response);
            _context.Language = ELanguage.Pi;
            WriteLine($"Recv: {response}");
            Assert.IsTrue(_context.Exec(response));
            var stack = _context.Executor.DataStack;
            WriteLine($"Recv: {stack}");
            Assert.AreEqual(1, stack.Count);
            Assert.AreEqual(3, stack.Pop());
        }

        public interface I007
        {
            int GetNumber();
        }

        public interface IAgent007
            : IAgent<IAgent007>
        {
            int GetNumber();
        }

        public interface IProxy007
            : IProxy<IProxy007>
        {
            NetId Id { get; }
            IPeer Peer { get; }
            IFuture<int> GetNumber();
        }

        public class Proxy007
            : IProxy007
        {
            public NetId Id { get; }
            public IPeer Peer { get; set; }

            public IFuture<int> GetNumber()
            {
                return Peer.RemoteCall<int>(Id, "GetNumber");
            }
        }

        [Test]
        public void TestAgents()
        {
            var peer = Network.Create.NewPeer(ListenPort);
            Assert.IsTrue(peer.StartSelfHosting(), peer.Error);

            IAgent007 agent = peer.NewAgent<IAgent007>();

            IEnumerator Gen(IGenerator self)
            {
                var proxy = peer.NewProxy<IProxy007>(agent.NetId);
                yield return self.ResumeAfter(proxy);
                Assert.IsTrue(proxy.Available);
                var number = proxy.Value.GetNumber();
                yield return self.ResumeAfter(number);
                Assert.AreEqual(42, number.Value);
            }

            var kernel = Flow.Create.Kernel();
            kernel.Root.Add(kernel.Factory.Coroutine(Gen));
            kernel.Step();
            kernel.Step();
            kernel.Step();
        }
    }
}
