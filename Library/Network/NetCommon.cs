
using Diver.Exec;

namespace Diver.Network
{
    public class NetCommon : NetworkConsoleWriter
    {
        protected Peer _peer;
        protected Flow.IFactory _factory => _peer.Factory;
        protected Executor _exec => _peer.Executor;

        public NetCommon(Peer peer)
        {
            _peer = peer;
        }
    }
}