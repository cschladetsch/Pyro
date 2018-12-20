using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Diver.LanguageCommon
{
    /// <summary>
    /// Common to all custom language's Lexer, Parser, and Translator processes
    /// </summary>
    public class Process
    {
        public bool Failed => _failed;
        public string Error => _error;

        protected Process()
        {
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

        private bool _failed;
        protected string _error;
    }
}
