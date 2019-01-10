using System;
using System.Reflection;
using System.Text;
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

            Con.CancelKeyPress += Cancel;

            WriteHeader();

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

        private void Exit(int result = 0)
        {
            Con.ForegroundColor = _originalColor;
            Environment.Exit(result);
        }

        private static void Cancel(object sender, ConsoleCancelEventArgs e)
        {
            // don't exit immediately - shut down networking gracefully first
            e.Cancel = true;
            _self.Shutdown();
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
            Exit();
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

                _peer.Continue(cont);
                return true;
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

        private readonly Peer _peer;
        private readonly Context _context;
        private static Program _self;
        private readonly ConsoleColor _originalColor;
        private string HostName => _peer.HostName;
        private int HostPort => _peer.HostPort;
    }
}
