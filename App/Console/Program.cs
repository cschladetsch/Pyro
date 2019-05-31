using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Pyro.ExecutionContext;
using Pyro.Language;
using Pyro.Network;

namespace Pyro.Console
{
    using Con = System.Console;

    internal class Program : AppCommon.AppCommonBase
    {
        public const int ListenPort = 9999;

        private readonly Context _context;
        private IPeer _peer;
        private string HostName => _peer.Remote?.HostName;
        private int HostPort => _peer.Remote?.HostPort ?? 0;

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

                return _peer.Execute(cont);
            }
            catch (Exception e)
            {
                Error(e.Message);
            }

            return false;
        }

        public void WriteDataStackContents(IClient client, int max = 50)
        {
            Con.ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            var results = client.Results().ToList();
            var n = 0;
            foreach (var result in results)
                str.AppendLine($"{n++}: {result}");

            Con.Write(str.ToString());
        }

        private static void Main(string[] args)
        {
            new Program(args).Repl();
        }

        private Program(string[] args)
            : base(args)
        {
            _context = new Context();

            if (!StartPeer(args))
                Exit(1);

            RunInitialisationScripts();

            _peer.OnReceivedResponse += (server, client, text) => WriteLine(text, ConsoleColor.Magenta);
        }

        private static string GetVersion()
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            var version = name.Version;
            var built = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.MinorRevision * 2);
            return $"Pyro {name.Name} {version} built {built}";
        }

        private bool StartPeer(string[] args)
        {
            var port = ListenPort;
            if (args.Length == 1 && !int.TryParse(args[0], out port))
                return Error("Local server listen port number expected as argument");

            _peer = Network.Create.NewPeer(port);
            return _peer.StartSelfHosting() || Error("Failed to start local server");
        }

        private void RunInitialisationScripts()
        {
            // TODO: run things like ~/.pyro-start.{pi|rho}
        }

        private void WriteHeader()
        {
            Write($"{GetVersion()}\n", ConsoleColor.DarkGray);
        }

        private void Repl()
        {
            while (true)
            {
                try
                {
                    WritePrompt();
                    var input = GetInput();
                    if (string.IsNullOrEmpty(input))
                        _peer.Remote.Continue("1 drop");    // hack to force stack refresh!
                    else if (!Execute(input))
                        continue;
                    
                    System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(25));

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

        private void WriteDataStack()
        {
            var current = Con.ForegroundColor;
            WriteDataStackContents(_peer.Remote);
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

To do both:
...rho> join(""hostname""[, port])

For help on syntax for Pi/Rho languages, see the corresponding documentation.

Press Ctrl-C to quit.
");
            return true;
        }

        protected override void Shutdown()
        {
            var color = ConsoleColor.DarkGray;
            Error("Shutting down...", color);
            _peer?.Stop();
            Error("Done", color);
            Con.ForegroundColor = ConsoleColor.White;
            Exit();
        }
    }
}
