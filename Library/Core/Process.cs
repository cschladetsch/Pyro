using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Diver
{
    /// <summary>
    /// Common to all custom language's Lexer, Parser, and Translator processes
    /// </summary>
    public class Process : IProcess
    {
        public bool Failed => _failed;
        public string Error
        {
            get => _error;
            protected set => _error = value;
        }

        protected Process()
        {
        }

        public virtual bool Fail(string err)
        {
            _failed = true;
            _error = err;
            return false;
        }

        public virtual bool Fail(string fmt, params object[] args)
        {
            return Fail(string.Format(fmt, args));
        }

        protected void Reset()
        {
            _failed = false;
            _error = "";
        }

        private bool _failed;
        protected string _error;
    }
}
