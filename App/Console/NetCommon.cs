namespace Console
{
    public class NetCommon : NetworkConsoleWriter
    {
        protected Peer _peer;
        protected Flow.IFactory _factory => _peer.Factory;

        public NetCommon(Peer peer)
        {
            _peer = peer;
        }
    }
}