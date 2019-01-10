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
            WriteHeader();

            Con.CancelKeyPress += Cancel;

            var port = ListenPort;
            if (args.Length == 1)
                port = int.Parse(args[0]);

            _peer = new Peer(port);
            _peer.Start();
            if (!_peer.Connect(_peer.GetLocalHostname(), ListenPort))
            {
                Error("Couldn't connect to local host");
                Exit(1);
            }

            //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

            if (!_peer.EnterRemote(_peer.Clients[0]))
            {
                Error("Couldn't shell to local");
                Exit(1);
            }

            _context = new Context();
        }

        private static void Exit(int result = 0)
        {
            Environment.Exit(result);
        }

        private static void Cancel(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _self.Shutdown();
        }

        private void WriteHeader()
        {
            Con.ForegroundColor = ConsoleColor.DarkGray;
            Con.WriteLine($"{GetVersion()}");
        }

        private static string GetVersion()
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            var version = name.Version;
            var built = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.MinorRevision * 2);
            return $"{name.Name} {version} built {built}";
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

        private void Shutdown()
        {
            var color = ConsoleColor.DarkGray;
            Error("Shutting down...", color);
            _peer?.Stop();
            Error("Done", color);
            Con.ForegroundColor = ConsoleColor.White;
            Environment.Exit(0);
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
                switch (input)
                {
                    case "help":
                    case "?":
                        return ShowHelp();
                    case "rho":
                        _context.Language = Pyro.ExecutionContext.ELanguage.Rho;
                        return true;
                    case "pi":
                        _context.Language = Pyro.ExecutionContext.ELanguage.Pi;
                        return true;
                }

                if (!_context.Translate(input, out var cont))
                    return Error(_context.Error);

                _peer.Continue(cont);
                return true;
            }
            catch (Exception e)
            {
                Error(e.Message);
            }

            return false;
        }

        private static bool ShowHelp()
        {
            Con.WriteLine(
@"
Before the prompt is printed, the data-stack of the 
contextual executor is printed. Operations you perform act on this
data-stack.

The prompt shows the current executing context and language.
When you type at the prompt, your text is translated to Pi script
and executed in current context.

To connect to a remote node, type:
...rho> peer.Connect(""_hostName"", port)

To switch execution context, type:
...rho> peer.Switch(""hostname"")

Press Ctrl-C to quit.
"
                );
            return true;
        }

        private void WritePrompt()
        {
            Con.ForegroundColor = ConsoleColor.DarkGray;
            Con.Write($"{_hostName}:{_hostPort} ");
            Con.ForegroundColor = ConsoleColor.Gray;
            Con.Write(MakePrompt());
            Con.ForegroundColor = ConsoleColor.White;
        }

        private string MakePrompt()
        {
            return $"{_context.Language}> ";
        }

        private static bool Error(string text, ConsoleColor color = ConsoleColor.Green)
        {
            Con.ForegroundColor = color;
            Con.WriteLine(text);
            return false;
        }

        private void WriteDataStack()
        {
            _peer.Remote?.WriteDataStackContents();
        }

        private readonly Peer _peer;
        private readonly Context _context;
        private string _hostName => _peer.HostName;
        private int _hostPort => _peer.HostPort;
        private static Program _self;
    }
}
