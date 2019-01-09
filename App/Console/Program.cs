using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Diver;
using Diver.Exec;
using Diver.Language;
using Diver.Network;
using Con = System.Console;

namespace Console
{
    class Program
    {
        public const int ListenPort = 9999;
        
        static void Main(string[] args)
        {
            _self = new Program(args);
            _self.Repl();
        }

        private Program(string[] args)
        {
            WriteHeader();

            var port = ListenPort;
            if (args.Length == 1)
                port = int.Parse(args[0]);

            Con.CancelKeyPress += Cancel;
            _registry = new Diver.Impl.Registry();
            _exec = _registry.Add(new Executor()).Value;
            _piTranslator = new PiTranslator(_registry);
            _rhoTranslator = new RhoTranslator(_registry);
            AddTypes(_registry);

            _peer = new Peer(port);
            _peer.Start();

            _exec.Scope["peer"] = _peer;
            _exec.Scope["con"] = this;
            _piTranslator.Translate(@"""192.168.56.1"" 'Connect peer .@ &");
            _exec.Scope["connect"] = _piTranslator.Result;

            _translator = _piTranslator;

            _hostPort = 0;
            _hostName = _peer.GetLocalHostname();
        }

        private static void Cancel(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _self.Shutdown();
        }

        private void WriteHeader()
        {
            Con.ForegroundColor = ConsoleColor.DarkGray;
            Con.WriteLine($"Console {GetVersion()}");
        }

        private static string GetVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        private void AddTypes(IRegistry registry)
        {
            registry.Register(new ClassBuilder<Program>(registry)
                .Methods
                    .Add<string, bool>("Execute", (q, s) => q.Execute(s))
                .Class);
            registry.Register(new ClassBuilder<Peer>(registry)
                .Methods
                    .Add<string, int, bool>("Connect", (q, s, p) => q.Connect(s, p))
                    .Add<Socket, bool>("Disconnect", (q, s) => q.Disconnect(s))
                .Class);
            registry.Register(new ClassBuilder<Client>(registry)
                .Methods
                    .Add<string, bool>("SendPi", (q, s) => q.SendPi(s))
                .Class);
        }

        private void Repl()
        {
            while (true)
            {
                try
                {
                    Process();
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

        private void Process()
        {
            WritePrompt();
            if (!Execute(GetInput()))
                return;
            WriteDataStack();
        }

        private string GetInput()
        {
            return Con.ReadLine();
        }

        public bool Execute(string input)
        {
            if (input == null)
                return false;

            try
            {
                if (input == "help" || input == "?")
                    return ShowHelp();

                if (input == "rho")
                {
                    _translator = _rhoTranslator;
                    return true;
                }

                if (input == "pi")
                {
                    _translator = _piTranslator;
                    return true;
                }

                if (!_translator.Translate(input))
                {
                    Error($"{_translator.Error}");
                    return false;
                }

                _exec.Continue(_translator.Result);
                return true;
            }
            catch (Exception e)
            {
                Error(e.Message);
            }

            return false;
        }

        private bool ShowHelp()
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
...> peer.Connect(""_hostName"", port)

To switch execution context, type:
...> peer.Switch(""hostname"")

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
            var lang = _translator == _piTranslator ? "pi" : "rho";
            return $"{lang}> ";
        }

        void Error(string text, ConsoleColor color = ConsoleColor.Green)
        {
            Con.ForegroundColor = color;
            Con.WriteLine(text);
        }

        private void WriteDataStack()
        {
            WriteDataStackContents();
        }

        public void WriteDataStackContents(int max = 20)
        {
            Con.ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            var data = _exec.DataStack.ToArray();
            max = Math.Min(data.Length, max);
            for (var n = max - 1; n >= 0; --n)
            {
                var obj = data[n];
                str.AppendLine($"{n}: {Print(obj)}");
            }
            Con.Write(str.ToString());
        }

        private string Print(object obj)
        {
            switch (obj)
            {
                case string str:
                    return $"\"{str}\"";
                case List<object> list:
                    var sb = new StringBuilder();
                    sb.Append('[');
                    var comma = "";
                    foreach (var elem in list)
                    {
                        sb.Append(comma + Print(elem));
                        comma = ", ";
                    }
                    sb.Append(']');
                    return sb.ToString();
            }

            return obj.ToString();
        }

        private readonly IRegistry _registry;
        private readonly Executor _exec;
        private readonly PiTranslator _piTranslator;
        private RhoTranslator _rhoTranslator;
        private ITranslator _translator;
        private Peer _peer;
        private bool IsPi => _translator == _piTranslator;
        private bool IsRho => _translator == _rhoTranslator;
        private static Program _self;

        private string _hostName;
        private int _hostPort;
    }
}
