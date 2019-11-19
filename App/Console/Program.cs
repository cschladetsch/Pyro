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
            {
                var reg = _peer.Local.Context.Registry;
                reg.Register(new ClassBuilder<UserClass>(reg).Class);
                
                // not strictly needed, but avoids runtime reflection on calls
                new ClassBuilder<TestClient>(reg)
                    .Methods
                        .Add<int, float, float, float>("UpdateTransform", (q, n, x, y, z) => q.UpdateTransform(n, x, y, z))
                    ;

                var scope = _peer.Local.Context.Executor.Scope;
                scope["remote"] = new TestClient();

                _peer.OnConnected += OnConnected;

                _peer.OnReceivedRequest
                    += (client, text)
                        => WriteLine(text, ConsoleColor.Magenta);
            }
        }

        /// <summary>
        /// Invoked server-side when a new client connects
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="client"></param>
        private void OnConnected(IPeer peer, IClient client)
        {
            //WriteLine("OnConnected: adding methods");
            var scope = peer.Local.Context.Executor.Scope;
            scope["Connected"] = Pyro.Create.Method<TestClient, string, int>((q,name,id) => q.Connected(name, id));
            scope["UpdateTransform"] = Pyro.Create.Method<TestClient, int,float,float,float>((q,id,x,y,z) => q.UpdateTransform(id,x,y,z));
            scope["Disconnected"] = Pyro.Create.Method<TestClient, int>((q,id) => q.Disconnect(id));
            
        }

        private bool Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                //WriteDataStack();
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

        private void WriteLocalDataStack(int max = 50)
        {
            Con.ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            var results = _context.Executor.DataStack;
            var n = 0;
            foreach (var result in results)
            {
                str.AppendLine($"{n++}: {_context.Registry.ToPiScript(result)}");
                if (n >= max)
                    break;
            }

            Con.Write(str.ToString());
        }

        private static void WriteDataStackContents(IClient client, int max = 50)
        {
            Con.ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            var results = client.Results().ToList();
            var n = results.Count - 1;
            foreach (var result in results)
                str.AppendLine($"{n--}: {result}");

            Con.Write(str.ToString());
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

        private bool StartPeer(string[] args)
        {
            var port = ListenPort;
            if (args.Length == 1 && !int.TryParse(args[0], out port))
                return Error("Local server listen port number expected as argument");

            _peer = Create.NewPeer(port);
            //_context.Executor.Scope["local"] = _peer;
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
    }
}

