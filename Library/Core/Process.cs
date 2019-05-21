namespace Pyro
{
    /// <summary>
    /// This is a simple class which works in various contexts.
    /// It is common to all things that preservce state over time, or `Processes`.
    /// Notablety each language's Lexer, Parser, and Translator are all Proceses's.
    /// The base of all netork classes are also Process's.
    /// </summary>
    public class Process : IProcess
    {
        public bool Failed { get; private set; }

        protected string _error;
        
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
            Failed = true;
            _error = err;
            return false;
        }

        protected void Reset()
        {
            Failed = false;
            _error = "";
        }
    }
}
