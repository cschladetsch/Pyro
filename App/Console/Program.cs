using System;
using System.Reflection;

using Diver.Network;
using Pyro.ExecutionContext;

using Con = System.Console;

namespace Console
{
    internal class Program
    {
        public const int ListenPort = 9999;

        private static void Main(string[] args)
        {
            _self = new Program(args);
            _self.Repl();
        }

        private Program(string[] args)
        {
            _originalColor = Con.ForegroundColor;
            _context = new Context();
            Con.CancelKeyPress += Cancel;

            WriteHeader();
            if (!StartPeer(args))
                Exit(1);

            RunInitialisationScripts();
        }

        private bool StartPeer(string[] args)
        {
            var port = ListenPort;
            if (args.Length == 1 && !int.TryParse(args[0], out port))
                return Error("Local server listen port number expected as argument");

            _peer = new Peer(port);
            if (!_peer.Start())
                return Error("Failed to start local server");

            if (!_peer.Connect(_peer.GetLocalHostname(), port)) 
                return Error("Couldn't connect to localhost");

            // unsure if truly needed, but this Sleep is to give a little time for local
            // client to connect to local server via Tcp
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(.2));

            return _peer.EnterRemote(_peer.Clients[0]) || Error("Couldn't shell to localhost");
        }

        private void RunInitialisationScripts()
        {
            // TODO: run things like ~/.pyro-start.{pi|rho}
        }

        private void WriteHeader()
        {
            Write($"{GetVersion()}\n", ConsoleColor.DarkGray);
        }

        private static string GetVersion()
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            var version = name.Version;
            var built = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.MinorRevision * 2);
            return $"Pyro {name.Name} {version} built {built}";
        }

        private void Repl()
        {
            while (true)
            {
                try
                {
                    WritePrompt();
                    if (!Execute(GetInput()))
                        continue;
                    WriteDataStack();
                }
                catch (Exception e)
                {
                    Error($"{e.Message}");
                }
            }
        }

        private string GetInput()
        {
            return Con.ReadLine();
        }

        public bool Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            try
            {
                if (PreProcess(input))
                    return true;

                if (!_context.Translate(input, out var cont))
                    return Error(_context.Error);

                return _peer.Continue(cont);
            }
            catch (Exception e)
            {
                Error(e.Message);
            }

            return false;
        }

        private bool PreProcess(string input)
        {
            switch (input)
            {
                case "help":
                case "?":
                    return ShowHelp();
                case "rho":
                    _context.Language = ELanguage.Rho;
                    return true;
                case "pi":
                    _context.Language = ELanguage.Pi;
                    return true;
            }

            return false;
        }

        private void WritePrompt()
        {
            Write($"{HostName}:{HostPort} ", ConsoleColor.DarkGray);
            Write($"{_context.Language}> ", ConsoleColor.Gray);
            Con.ForegroundColor = ConsoleColor.White;
        }

        private void Exit(int result = 0)
        {
            Con.ForegroundColor = _originalColor;
            Environment.Exit(result);
        }

        private static bool Error(string text, ConsoleColor color = ConsoleColor.Green)
        {
            Write(text, color);
            return false;
        }

        private static void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            var current = Con.ForegroundColor;
            Con.ForegroundColor = color;
            Con.Write(text);
            Con.ForegroundColor = current;
        }

        private void WriteDataStack()
        {
            var current = Con.ForegroundColor;
            _peer.Remote?.WriteDataStackContents();
            Con.ForegroundColor = current;
        }

        private static bool ShowHelp()
        {
            Con.WriteLine(
@"
The prompt shows the current executing context and language.

When you type at the prompt, your text is executed in the current context - which could be local (default) or remote.

Before the prompt is printed, the data-stack of the current server is printed. Operations you perform act on this data-stack. Each connection to a remote server has its own private context.

To connect to a remote node, type:
...rho> connect(""_hostName""[, port])

To then switch execution context, type:
...rho> enter(""hostname"")

To do both consequetively:
...rho> join(""hostname""[, port])

For help on syntax for Pi/Rho languages, see the corresponding documentation.

Press Ctrl-C to quit.
"
            );
            return true;
        }

        private static void Cancel(object sender, ConsoleCancelEventArgs e)
        {
            // don't exit immediately - shut down networking gracefully first
            e.Cancel = true;
            _self.Shutdown();
        }

        private void Shutdown()
        {
            var color = ConsoleColor.DarkGray;
            Error("Shutting down...", color);
            _peer?.Stop();
            Error("Done", color);
            Con.ForegroundColor = ConsoleColor.White;
            Exit();
        }

        private Peer _peer;
        private readonly Context _context;
        private readonly ConsoleColor _originalColor;
        private static Program _self;
        private string HostName => _peer.HostName;
        private int HostPort => _peer.HostPort;
    }
}
