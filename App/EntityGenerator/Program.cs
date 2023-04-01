// (C) 2023 christian.schladetsch@gmail.com. All rights reserved.

namespace Pyro.Console {
    using SystemConsole = System.Console;

    /// <summary>
    ///     A Repl console for Pyro.
    ///     Can connect and enter into other consoles.
    /// </summary>
    internal class Program {
        // Port that we listen on for incoming connections.
        private const int ListenPort = 7777;

        // only used for translation - not execution. Execution is performed by servers or clients.private readonly ExecutionContext _context = new 
        private ExecutionContext.ExecutionContext _context = new ExecutionContext.ExecutionContext();


        private Program(string[] args) {
        }

        public static void Main(string[] args) {
            new Program(args).Run();
        }

        private void Run() {
            throw new NotImplementedException();
        }
    }
}
