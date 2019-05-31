using Pyro.Network.Impl;

namespace Pyro.Network
{
    public static class RegisterTypes
    {
        public static void Register(IRegistry registry)
        {
            Exec.RegisterTypes.Register(registry);
            Peer.Register(registry);
        }
    }
}

