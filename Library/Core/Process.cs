using System;

namespace Pyro
{
    /// <inheritdoc />
    /// <summary>
    /// This is a simple class which works in various contexts.
    ///
    /// It is common to all things that preserve state over time.
    /// 
    /// Notably each language's Lexer, Parser, and Translator are all Proceses's.
    ///
    /// The base of all netork classes are also Process's.
    ///
    /// This is the work-horse of many systems. A very humble yet very important and used class.
    /// </summary>
    public class Process
        : IProcess
    {
        public bool Failed { get; private set; }
        public string Error { get; protected set; }

        /// <summary>
        /// A special kind of failure: this is a failure of the system itself,
        /// rather than a failure to produce results given invalid user input.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        protected virtual bool InternalFail(string error)
        {
            var text = $"INTERNAL: {error}";
            Fail(text);
            throw new Exception(text);
        }

        /// <summary>
        /// A Process failure due to client input.
        /// </summary>
        /// <param name="err">What went wrong.</param>
        /// <returns>false.</returns>
        protected virtual bool Fail(string err)
        {
            Failed = true;
            Error = err;
            return false;
        }

        /// <summary>
        /// Reset this Process to a successful state.
        /// </summary>
        public void Reset()
        {
            Failed = false;
            Error = "";
        }
    }
}

