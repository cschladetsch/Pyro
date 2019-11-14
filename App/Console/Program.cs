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
        {
            return a + b;
        }
    }

    internal class Program
        : AppCommon.AppCommonBase
    {
        public const int ListenPort = 7777;

        private readonly Context _context;
        private IPeer _peer;
        /// <summary>
        /// If true, start a local peer and use loopback
        /// </summary>
        private readonly bool _useLoopback = true;

        private string HostName => _peer?.Remote?.HostName ?? "local";
        private int HostPort => _peer?.Remote?.HostPort ?? 0;

        public static void Main(string[] args)
            => new Program(args).Repl();

        public Program(string[] args)
            : base(args)
        {
            _context = new Context();
            _context.Language = ELanguage.Rho;
            RegisterTypes.Register(_context.Registry);

            _context.Registry.Register(new ClassBuilder<UserClass>(_context.Registry).Class);

            if (_useLoopback && !StartPeer(args))
                Exit(1);

            RunInitialisationScripts();

            if (_peer != null)
            {
                var r = _peer.Local.Context.Registry;
                r.Register(new ClassBuilder<UserClass>(r).Class);

                _peer.OnReceivedRequest
                    += (server, client, text)
                        => WriteLine(text, ConsoleColor.Magenta);
            }
        }

        public bool Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                if (_peer != null)
                    WriteLocalDataStack();

                return true;
            }

            try
            {
                if (PreProcess(input))
                    return true;

                if (!_context.Translate(input, out var cont))
                    return Error(_context.Error);

                if (_peer != null)
                    _peer.Execute(cont);
                else
                {
                    cont.Scope = _context.Executor.Scope;
                    _context.Executor.Continue(cont);
                }
            }
            catch (Exception e)
            {
                Error(e.Message);
                if (_peer != null)
                    _peer.Execute($"Error: {e.Message} {e.InnerException?.Message}");
            }
            
            WriteLocalDataStack();
            
            return false;
        }

        public void WriteLocalDataStack(int max = 50)
        {
            Con.ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            var results = _context.Executor.DataStack;
            var n = 0;
            foreach (var result in results)
                str.AppendLine($"{n++}: {_context.Registry.ToPiScript(result)}");

            Con.Write(str.ToString());
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
                    {
                        _peer?.Remote?.Continue("1 drop");    // hack to force stack refresh!
                        System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(25));
                    }
                    else if (!Execute(input))
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
rho> connect(""_hostName""[, port])

To then switch execution context, type:
rho> enter(""hostname"")

To do both:
rho> join(""hostname""[, port])

For help on syntax for Pi/Rho languages, see the corresponding documentation.

Press Ctrl-C to quit.
");
            return true;
        }
    }
}

