using System.Runtime.InteropServices;
using Diver.Core;

namespace Diver
{
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

        public IRef<T> New<T>()
        {
            return _registry.New<T>();
        }

        public IRef<T> New<T>(T val)
        {
            var obj = _registry.New<T>();
            obj.Value = val;
            return obj;
        }

        private bool _failed;
        protected string _error;
        private IRegistry _registry;
    }
}
