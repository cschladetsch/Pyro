using System;
using System.Reflection;
using Diver;
using Diver.Impl;
using Diver.Exec;
using Diver.Language;
using Diver.Network;
using Con = System.Console;

namespace Console
{
    public enum ELanguage
    {
        None,
        Pi,
        Rho,
    }

    public class ExecutionContext : Process
    {
        public IRegistry Registry => _registry;
        public ITranslator Ttranslator => _translator;
        public Executor Executor => _exec;
        public ELanguage Language
        {
            get
            {
                if (_translator == _pi)
                    return ELanguage.Pi;
                return _translator == _rho ? ELanguage.Rho : ELanguage.None;
            }
            set
            {
                switch (value)
                {
                    case ELanguage.None:
                        _translator = null;
                        return;
                    case ELanguage.Pi:
                        _translator = _pi;
                        break;
                    case ELanguage.Rho:
                        _translator = _rho;
                        break;
                }
            }
        }

        public ExecutionContext()
        {
            _registry = new Registry();
            _exec = _registry.Add(new Diver.Exec.Executor()).Value;
            _pi = new PiTranslator(_registry);
            _rho = new RhoTranslator(_registry);
        }

        public bool Exec(string text)
        {
            if (_translator == null)
                return Fail("No translator");
            if (!_translator.Translate(text))
                return Fail(_translator.Error);
            try
            {
                var cont = _translator.Result;
                cont.Scope = _exec.Scope;
                _exec.Continue(_translator.Result);
            }
            catch (Exception e)
            {
                return Fail(e.Message);
            }
            return true;
        }

        public bool ExecFile(string fileName)
        {
            return Fail("Not Implemented");
        }

        private readonly IRegistry _registry;
        private readonly Executor _exec;
        private readonly PiTranslator _pi;
        private readonly RhoTranslator _rho;
        private ITranslator _translator;
    }

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
                Environment.Exit(1);
            }

            if (!_peer.EnterRemote(_peer.Clients[0]))
            {
                Error("Couldn't shell to local");
                Environment.Exit(1);
            }

            // needed to generate code locally for sending
            _registry = new Registry();
            _piTranslator = new PiTranslator(_registry);
            _rhoTranslator = new RhoTranslator(_registry);

            _translator = _piTranslator;
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
                        return;
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
            if (input == null)
                return false;

            try
            {
                switch (input)
                {
                    case "help":
                    case "?":
                        return ShowHelp();
                    case "rho":
                        _translator = _rhoTranslator;
                        return true;
                    case "pi":
                        _translator = _piTranslator;
                        return true;
                }

                if (!_translator.Translate(input))
                {
                    Error($"{_translator.Error}");
                    return false;
                }

                var continuation = _translator.Result;
                _peer.Continue(continuation);
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
            var lang = _translator == _piTranslator ? "pi" : "rho";
            return $"{lang}> ";
        }

        private static void Error(string text, ConsoleColor color = ConsoleColor.Green)
        {
            Con.ForegroundColor = color;
            Con.WriteLine(text);
        }

        private void WriteDataStack()
        {
            _peer.Remote?.WriteDataStackContents();
        }

        private readonly PiTranslator _piTranslator;
        private readonly RhoTranslator _rhoTranslator;
        private readonly Peer _peer;
        private ITranslator _translator;
        private bool IsPi => _translator == _piTranslator;
        private bool IsRho => _translator == _rhoTranslator;
        private IRegistry _registry;
        //private Executor _exec => _peer.Executor;
        private string _hostName => _peer.HostName;
        private int _hostPort => _peer.HostPort;
        private static Program _self;
    }
}
