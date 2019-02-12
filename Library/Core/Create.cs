namespace Pyro
{
    public static class Create
    {
        public static IFactory Factory = new Impl.Factory();

        public static IRegistry NewRegistry()
        {
            return Factory.NewRegistry();
        }
    }
}