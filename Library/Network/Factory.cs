namespace Pyro.Network
{
    /// <summary>
    /// TODO
    /// </summary>
    public static class Create
    {
        public static IPeer NewPeer(int port)
        {
            return new Impl.Peer(port);
        }
    }
}
