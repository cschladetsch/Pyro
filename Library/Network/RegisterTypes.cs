using Pyro.Network.Impl;

namespace Pyro.Network
{
    public static class RegisterTypes
    {
        public static void Register(IRegistry registry)
        {
            Exec.RegisterTypes.Register(registry);

            registry.Register(new ClassBuilder<Peer>(registry)
                .Methods
                    .Add<string, int, bool>("Connect", (q, s, p) => q.Connect(s, p))
                    .Add<Client, bool>("Remote", (q, s) => q.EnterRemote(s))
                    .Add<int, bool>("RemoteAt", (q, s) => q.EnterRemoteAt(s))
                    .Add<Client>("Leave", (q, s) => q.Leave())
                .Class);
            registry.Register(new ClassBuilder<Client>(registry)
                .Methods
                    //.Add<string, bool>("Send", (q, s) => q.Send(s))
                .Class);
        }

    }
}
