using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Pyro.Network;
using Pyro.Language;
using Pyro.AppCommon;
using Pyro.Network.Impl;

namespace Pyro.Console {
    using SystemConsole = System.Console;

    internal class Program
        : ConsoleAppCommonBase {
        // Port that we listen on for incoming connections.
        private const int _ListenPort = 7777;

        // only used for translation - not execution. Execution is performed by servers or clients.
        private readonly ExecutionContext.ExecutionContext _context;

        // our peer that can connect to any other peer, and can also be connected to by any other peer
        private IPeer _peer;

        // used to lock synchronise output
        private readonly object _consoleLock = new object();

        private Program(string[] args)
            : base(args) {
            _context = new ExecutionContext.ExecutionContext { Language = ELanguage.Rho };
            Factory.RegisterTypes(_context.Registry);

            if (!StartPeer(args)) {
                Exit(1);
            }

            AddPeerEventHandlers();

            RunInitialisationScripts();
        }

        public static void Main(string[] args) {
            new Program(args).Repl();
        }

        private void AddPeerEventHandlers() {
            _peer.OnConnected += OnConnected;
            _peer.OnReceivedRequest += HandleInputRequest;
        }

        private int n;

        private void HandleInputRequest(IPeer peer, IClient client, string text) {
            WriteLine(n + text, ConsoleColor.Magenta);
        }

        private bool StartPeer(IReadOnlyList<string> args) {
            var port = _ListenPort;
            if (args.Count == 1 && !int.TryParse(args[0], out port)) {
                return Error($"Local server listen port number expected as argument, got {args[0]}");
            }

            _peer = Factory.NewPeer(new Domain(), port);
            return _peer.SelfHost() || Error($"Failed to start local self-hosting peer: {_peer.Error}");
        }

        private void RunInitialisationScripts() {
            // TODO: run things like ~/.pyrorc.{pi,rho}
        }

        private void Repl() {
            while (true) {
                try {
                    WritePrompt();
                    if (!Execute(GetInput())) {
                        Error(_context.Error);
                    }

                    WriteDataStack();
                } catch (Exception e) {
                    Error($"{e.Message}");
                }
            }
        }

        private void WritePrompt() {
            lock (_consoleLock) {
                Write($"{_peer.Remote?.Socket?.RemoteEndPoint} ", ConsoleColor.DarkGray);
                Write($"{_context.Language}> ", ConsoleColor.Gray);
                SystemConsole.ForegroundColor = ConsoleColor.White;
            }
        }

        private static string GetInput() {
            return SystemConsole.ReadLine();
        }

        private bool PreProcessCommand(string input) {
            switch (input) {
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

        private bool Execute(string input) {
            if (string.IsNullOrEmpty(input)) {
                _peer.Remote?.Continue("nop");
                return true;
            }

            try {
                if (PreProcessCommand(input)) {
                    return true;
                }

                return !_context.Translate(input, out var cont)
                    ? Error(_context.Error) 
                    : _peer.Execute(cont.ToText());
            } catch (Exception e) {
                Error(e.Message);
                _peer.Execute($"Error: {e.Message} {e.InnerException?.Message}");
            }

            return false;
        }

        private void OnConnected(IPeer peer, IClient client) {
            WriteLine($"Connected to {client}.");
        }

        private void WriteDataStackContents(INetCommon client, int max = 50) {
            var stringBuilder = new StringBuilder();
            // Make a copy as it could be changed by another network call while we're iterating over data stack.
            var results = client.ExecutionContext.Executor.DataStack.ToList();
            var reg = client.ExecutionContext.Registry;
            var n = Math.Min(max, results.Count - 1);
            foreach (var result in results) {
                stringBuilder.AppendLine($"{n--}: {reg.ToPiScript(result)}");
            }

            var str = stringBuilder.ToString();
            if (!string.IsNullOrEmpty(str)) {
                Write(stringBuilder.ToString(), ConsoleColor.Yellow);
            }
        }

        private void WriteDataStack() {
            var current = SystemConsole.ForegroundColor;
            WriteDataStackContents(_peer.Remote);
            SystemConsole.ForegroundColor = current;
        }

        private static bool ShowHelp() {
            SystemConsole.WriteLine(
                @"
The prompt shows the current executing context and language.
When you type at the prompt, your text is executed in the current context - which could be local (default) or remote.
Before the prompt is printed, the data-stack of the current server is printed. Operations you perform act on this data-stack. Each connection to a remote server has its own private context.
To connect to a remote node, type:

Rho> peer.Connect(""hostName"", port) 

To then switch execution context, type:
Rho> peer.Enter(n) 

Press Ctrl-D or type 'leave' to leave current context and return to local context.
Press Ctrl-C to quit.
");
            return true;
        }

        protected override void Shutdown() {
            const ConsoleColor color = ConsoleColor.DarkGray;
            Error("Shutting down...", color);
            _peer?.Stop();
            Error("Done", color);
            SystemConsole.ForegroundColor = ConsoleColor.White;
            Exit();
        }
    }
}