namespace Pyro.Console
{
    using System;
    using System.Linq;
    using System.Text;
    using ExecutionContext;
    using Language;
    using Network;
    using Con = System.Console;

    public class UserClass
    {
        public string Name;
        
        public int Add(int a, int b)
            => a + b;
    }

    /// <summary>
    /// A Repl console for Pyro.
    /// 
    /// Can connect and enter into other consoles.
    /// </summary>
    internal class Program
        : AppCommon.AppCommonBase
    {
        /// <summary>
        /// Port that we listen on for incoming connections.
        /// </summary>
        private const int ListenPort = 7777;

        private IPeer _peer;
        
        // only used for translation
        private readonly Context _context;

        /// <summary>
        /// If true, start a local peer and use loopback Tcp to local server.
        /// </summary>
        private readonly bool _useLoopback = true;

        private string HostName => _peer?.Remote?.HostName ?? "local";
        private int HostPort => _peer?.Remote?.HostPort ?? 0;

        public static void Main(string[] args)
            => new Program(args).Repl();

        private Program(string[] args)
            : base(args)
        {
            _context = new Context { Language = ELanguage.Rho };
            RegisterTypes.Register(_context.Registry);

            _context.Registry.Register(new ClassBuilder<UserClass>(_context.Registry).Class);

            if (_useLoopback && !StartPeer(args))
                Exit(1);

            RunInitialisationScripts();

            if (_peer != null)
                CreatePeer();
        }

        private void CreatePeer()
        {
            _peer.OnConnected += OnConnected;
            _peer.OnReceivedRequest += (client, text) => WriteLine(text, ConsoleColor.Magenta);
        }

        private bool StartPeer(string[] args)
        {
            var port = ListenPort;
            if (args.Length == 1 && !int.TryParse(args[0], out port))
                return Error($"Local server listen port number expected as argument, got {args[0]}");

            _peer = Create.NewPeer(port);
            var scope = _peer.Local.Context.Executor.Scope;
            var reg = _peer.Local.Context.Registry;
            reg.Register(new ClassBuilder<TestClient>(reg).Class);
            return _peer.SelfHost() || Error("Failed to start local server");
        }

        private void RunInitialisationScripts()
        {
            // TODO: run things like ~/.pyrorc.{pi,rho}
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

        private void WritePrompt()
        {
            Write($"{_peer.Remote?.Socket?.RemoteEndPoint} ", ConsoleColor.DarkGray);
            Write($"{_context.Language}> ", ConsoleColor.Gray);
            Con.ForegroundColor = ConsoleColor.White;
        }

        private static string GetInput()
            => Con.ReadLine();

        private bool PreProcess(string input)
        {
            // Idea here was to allow execution of arbitrary commands in the current context.
            // This is a wonderfully stupid idea for security reasons, but still.
            // In any case, I've removed it for now.
            //
            // if (!string.IsNullOrEmpty(input))
            // {
            //    if (input.StartsWith("."))
            //    {
            //        //var cmd = input.Substring(1);
            //        var bash = @"C:\Program Files\Git\git-bash.exe";
            //        var proc = new System.Diagnostics.Process();
            //        proc.OutputDataReceived += (sender, args) => WriteLine(args.Data);
            //        ProcessStartInfo si = new ProcessStartInfo();
            //        si.RedirectStandardOutput = true;
            //        si.UseShellExecute = false;
            //        si.CreateNoWindow = true;
            //        si.RedirectStandardOutput = true;
            //        si.FileName = bash;
            //        //si.Arguments = cmd;
            //        proc.StartInfo = si;
            //        var sr =
            //        proc.StandardOutput =
            //        return true;
            //    }
            // }

            switch (input)
            {
                case "?":
                case "help":
                    return ShowHelp();
                case "rho":
                    _context.Language = ELanguage.Rho;
                    return true;
                case "pi":
                    _context.Language = ELanguage.Pi;
                    return true;
                case "leave":
                    _peer.Leave();
                    return true;
            }

            return false;
        }

        /// <summary>
        /// First, we translate the input (which could be any supported language) to Pi.
        /// Then we convert that to Pi text and send to current server.
        /// </summary>
        private bool Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                // hack to get refresh of remote data stack
                _peer?.Remote?.Continue(" ");
                return true;
            }

            try
            {
                if (PreProcess(input))
                    return true;

                if (!_context.Translate(input, out var cont))
                    return Error(_context.Error);

                if (_peer != null)
                    return _peer.Execute(cont.ToText());

                cont.Scope = _context.Executor.Scope;
                _context.Executor.Continue(cont);

                return true;
            }
            catch (Exception e)
            {
                Error(e.Message);
                _peer?.Execute($"Error: {e.Message} {e.InnerException?.Message}");
            }
            
            return false;
        }

        /// <summary>
        /// Invoked server-side when a new client connects
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="client"></param>
        private void OnConnected(IPeer peer, IClient client)
        {
//            var scope = peer.Local.Context.Scope;
//            scope["remote"] = new TestClient();
//            client.Context.Executor.Scope["remote"] = new TestClient();
        }

        private void WriteLocalDataStack(int max = 50)
        {
            Con.ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            var results = _context.Executor.DataStack;
            var n = Math.Max(results.Count, max);
            foreach (var result in results)
            {
                str.AppendLine($"{--n}: {_context.Registry.ToPiScript(result)}");
                if (n == 0)
                    break;
            }

            Con.Write(str.ToString());
        }

        public static void WriteDataStackContents(IClient client, int max = 50)
        {
            Con.ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            // make a copy as it could be changed by another call while we're iterating over data stack
            var results = client.Context.Executor.DataStack.ToList();
            var reg = client.Context.Registry;
            var n = results.Count - 1;
            foreach (var result in results)
                str.AppendLine($"{n--}: {reg.ToPiScript(result)}");

            Con.Write(str.ToString());
        }

        private void WriteDataStack()
        {
            var current = Con.ForegroundColor;
            if (_peer != null)
                WriteDataStackContents(_peer.Remote);
            else
                WriteLocalDataStack();
            Con.ForegroundColor = current;
        }

        private bool ShowHelp()
        {
            Con.WriteLine(
@"
The prompt shows the current executing context and language.

When you type at the prompt, your text is executed in the current context - which could be local (default) or remote.

Before the prompt is printed, the data-stack of the current server is printed. Operations you perform act on this data-stack. Each connection to a remote server has its own private context.

To connect to a remote node, type:
Rho> connect(""_hostName"", port) // adds a connection to a remote server. all peers are servers and clients.

To then switch execution context, type:
Rho> enter(N) // where N is the client connection you want to enter

To do both:
Rho> join(""hostname"", port)

For help on syntax for Pi/Rho languages, see https://github.com/cschladetsch/Pyro

Press Ctrl-D to leave current context.

Press Ctrl-C to quit.
");
            return true;
        }

        protected override void Shutdown()
        {
            const ConsoleColor color = ConsoleColor.DarkGray;
            Error("Shutting down...", color);
            _peer?.Stop();
            Error("Done", color);
            Con.ForegroundColor = ConsoleColor.White;
            Exit();
        }
    }
}

