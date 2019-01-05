using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Diver;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;
using System.Threading;
using Flow.Impl;
using NetworkCommsDotNet;
using Con = System.Console;

namespace Console
{
    class Program
    {
        public const int ListenPort = 9999;
        private static bool _cancelled;
        private static Program _self;
        
        static void Main(string[] args)
        {
            _self = new Program(args);
            _self.Repl();
        }

        private Program(string[] args)
        {
            WriteHeader();
            Con.CancelKeyPress += Cancel;
            _registry = new Registry();
            _exec = new Executor(_registry);
            _piTranslator = new PiTranslator(_registry);
            _rhoTranslator = new RhoTranslator(_registry);
            AddTypes(_registry);

            _serverThread = new Thread(() => _peer = new Peer(_exec, ListenPort));
            _serverThread.Start();
        }

        private static void Cancel(object sender, ConsoleCancelEventArgs e)
        {
            // don't cancel immediately; let the app have a chance to close down gracefully
            e.Cancel = true;
            _cancelled = true;
            _self.Shutdown();
        }

        private void WriteHeader()
        {
            Con.ForegroundColor = ConsoleColor.DarkGray;
            Con.WriteLine($"Console {GetVersion()}\n");
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
            Error("Shutting down...");
            _peer?.Close();
            if (_serverThread.IsAlive)
                _serverThread.Join(TimeSpan.FromSeconds(2));
            Error("Done");
            Environment.Exit(0);
        }

        private void Process()
        {
            WritePrompt();
            var input = Con.ReadLine();
            if (!Execute(input))
                return;
            WriteDataStack();
        }

        public bool Execute(string input)
        {
            if (input == null)
                return false;

            try
            {
                if (!_piTranslator.Translate(input))
                {
                    Error($"{_piTranslator.Error}");
                    return false;
                }

                _exec.Continue(_piTranslator.Result());
                return true;
            }
            catch (Exception e)
            {
                Error(e.Message);
            }

            return false;
        }

        private void WritePrompt()
        {
            Con.ForegroundColor = ConsoleColor.Gray;
            Con.Write(MakePrompt());
            Con.ForegroundColor = ConsoleColor.White;
        }

        void Error(string text)
        {
            Con.ForegroundColor = ConsoleColor.Red;
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
            Con.WriteLine(str.ToString());
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

        private string MakePrompt()
        {
            return "pi> ";
        }

        private readonly IRegistry _registry;
        private readonly Executor _exec;
        private readonly PiTranslator _piTranslator;
        private readonly Thread _serverThread;
        private RhoTranslator _rhoTranslator;
        private Peer _peer;
    }
}
