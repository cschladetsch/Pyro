namespace Diver.LanguageCommon
{
    public class ProcessCommon : Process
    {
        protected ProcessCommon(IRegistry r)
        {
            _reg = r;
        }

        protected IRef<T> New<T>()
        {
            return _reg.Add<T>(default(T));
        }

        protected IRef<T> New<T>(T val)
        {
            return _reg.Add(val);
        }

        private readonly IRegistry _reg;
    }
}