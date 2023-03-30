using System;

namespace Pyro {
    /// <inheritdoc />
    public class Process
        : IProcess {
        public bool Failed { get; private set; }
        public string Error { get; protected set; }

        /// <summary>
        ///     Reset this Process to a successful state.
        /// </summary>
        public void Reset() {
            Failed = false;
            Error = "";
        }

        public virtual bool Run() {
            return Failed;
        }

        /// <summary>
        ///     A special kind of failure: this is a failure of the system itself,
        ///     rather than a failure to produce results given invalid user input.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        protected virtual bool InternalFail(string error) {
            var text = $"INTERNAL: {error}";
            Fail(text);
            throw new Exception(text);
        }

        /// <summary>
        ///     A Process failure due to client input.
        /// </summary>
        /// <param name="err">What went wrong.</param>
        /// <returns>false.</returns>
        public virtual bool Fail(string err) {
            Failed = true;
            Error = err;
            return false;
        }
    }
}