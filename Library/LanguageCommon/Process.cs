using System.Runtime.InteropServices;

namespace Diver
{
    /// <summary>
    /// Common to all custom language's Lexer, Parser, and Translator processes
    /// </summary>
    public class Process
    {
        public bool Failed => _failed;
        public string Error => _error;

        protected Process(IRegistry reg)
        {
            _registry = reg;
        }

        public bool Fail(string err)
        {
            _failed = true;
            _error = err;
            return false;
        }

        public bool Fail(string fmt, params object[] args)
        {
            return Fail(string.Format(fmt, args));
        }

        public IRef<T> New<T>() where T : class, new()
        {
            return _registry.Add<T>(new T());
        }

        public IRef<T> New<T>(T val)
        {
            return _registry.Add(val);
        }

        private bool _failed;
        protected string _error;
        private IRegistry _registry;
    }
}
