using System;
using System.Threading;
using System.Collections;

using NUnit.Framework;

using Flow;

using Pyro.Exec;
using Pyro.Network;
using Pyro.Language;
using Pyro.Network.Impl;

namespace Pyro.Test {
    [TestFixture]
    public class TestNetworkPeer
        : TestCommon {
        [SetUp]
        public override void Setup() {
            base.Setup();
            _context = new ExecutionContext.ExecutionContext();
        }

        public const int ListenPort = 8888;
        private ExecutionContext.ExecutionContext _context;

        [Test]
        public void TestSelfHosting() {
            var domain = new Domain();
            var peer = Factory.NewPeer(domain, ListenPort);
            peer.OnConnected += Connected;
            peer.OnReceivedRequest += Received;

            Assert.IsTrue(peer.SelfHost(), peer.Error);
            Assert.IsTrue(peer.Execute("{1 2 +}"), peer.Error);
            Settle();
            peer.Stop();
        }

        private static void Settle() {
            Thread.Sleep(TimeSpan.FromSeconds(0.2));
        }

        private void Connected(IPeer peer, IClient client) {
            WriteLine($"Test: Connected to {client}");
        }

        private void Received(IPeer self, IClient client, string request) {
            Assert.IsNotEmpty(request);
            _context.Language = ELanguage.Pi;
            WriteLine($"Recv: client={client}, req={request}");
            Assert.IsTrue(_context.Translate(request, out var cont));
            _context.Executor.Continue(cont.Code[0] as Continuation);
            var stack = _context.Executor.DataStack;
            Assert.AreEqual(1, stack.Count);
            Assert.AreEqual(3, stack.Pop());
        }

        public interface I007 {
            int GetNumber();
        }

        public interface IAgent007
            : IAgent<IAgent007>
                , IReflected {
            int GetNumber();
        }

        public interface IProxy007
            : IProxy<IProxy007> {
            NetId Id { get; }
            IPeer Peer { get; }
            IFuture<int> GetNumber();
        }

        public class Proxy007
            : IProxy007 {
            public NetId Id { get; }
            public IPeer Peer { get; set; }

            public IFuture<int> GetNumber() {
                return Peer.RemoteCall<int>(Id, "GetNumber");
            }
        }

        [Test]
        public void TestAgents() {
            var domain = new Domain();
            var peer = Factory.NewPeer(domain, ListenPort);
            Assert.IsTrue(peer.SelfHost(), peer.Error);

            var agent = peer.Domain.NewAgent<IAgent007>(null);

            IEnumerator Gen(IGenerator self) {
                var proxy = peer.Domain.NewProxy<IProxy007>(agent.NetId);
                yield return self.ResumeAfter(proxy);
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