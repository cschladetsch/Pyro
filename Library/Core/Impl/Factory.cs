namespace Pryo.Impl
{
    internal class Factory : IFactory
    {
        public IRegistry NewRegistry()
        {
            return new Registry();
        }
    }

}
