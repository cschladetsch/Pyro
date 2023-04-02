﻿using System;
using System.Linq;
using System.Text;
using Pyro.AppCommon;
using Pyro.Language;
using Pyro.Network;
using Pyro.Network.Impl;

namespace Pyro.Console {
    using SystemConsole = System.Console;

    /// <summary>
    ///     A Repl console for Pyro.
    ///     Can connect and enter into other consoles.
    /// </summary>
    internal class Program
        : ConsoleAppCommonBase {
        // Port that we listen on for incoming connections.
        private const int ListenPort = 7777;

        // only used for translation - not execution. Execution is performed by servers or clients.
        private readonly ExecutionContext.ExecutionContext _context;


        // our peer that can connect to any other peer, and can also be connected to by any other peer
        private IPeer _peer;

        private Program(string[] args)
            : base(args) {
            _context = new ExecutionContext.ExecutionContext { Language = ELanguage.Rho };
            Factory.RegisterTypes(_context.Registry);

            if (!StartPeer(args)) {
                Exit(1);
            }

            SetupPeer();

            RunInitialisationScripts();
        }

        public static void Main(string[] args) {
            new Program(args).Repl();
        }

        private void SetupPeer() {
            _peer.OnConnected += OnConnected;
            _peer.OnReceivedRequest += HandleInputRequest;
        }

        private void HandleInputRequest(IPeer _peer, IClient client, string text) {
            WriteLine(text, ConsoleColor.Magenta);
        }

        private bool StartPeer(string[] args) {
            var port = ListenPort;
            if (args.Length == 1 && !int.TryParse(args[0], out port)) {
                return Error($"Local server listen port number expected as argument, got {args[0]}");
            }

            _peer = Factory.NewPeer(new Domain(), port);
            var ctx = _peer.Local.ExecutionContext;
            var reg = ctx.Registry;
            var scope = ctx.Executor.Scope;

            /*reg.Register(new ClassBuilder<TestClient>(reg).Class);
            scope["remote"] = new TestClient();*/

            return _peer.SelfHost() || Error("Failed to start local server");
        }

        private void RunInitialisationScripts() {
            // TODO: run things like ~/.pyrorc.{pi,rho}
        }

        private void Repl() {
            while (true)
                try {
                    WritePrompt();
                    Execute(GetInput());
                    WriteDataStack();
                } catch (Exception e) {
                    Error($"{e.Message}");
                }
        }

        private readonly object _consoleLock = new object();

        private void WritePrompt() {
            lock (_consoleLock) {
                Write($"{_peer.Remote?.Socket?.RemoteEndPoint} ", ConsoleColor.DarkGray);
                Write($"{_context.Language}> ", ConsoleColor.Gray);
                SystemConsole.ForegroundColor = ConsoleColor.White;
            }
        }

        private string GetInput() {
            return SystemConsole.ReadLine();
            /* want to trap Ctrl-Keys
            var sb = new StringBuilder();
            while (true)
            {
                if (NetworkConsole.KeyAvailable)
                {
                    var ch = NetworkConsole.ReadKey();
                    if (ch.Key == ConsoleKey.Enter)
                    {
                        return sb.ToString();
                    }
                    
                    if (ch.Modifiers == ConsoleModifiers.Control)
                    {
                        switch (ch.Key)
                        {
                            case ConsoleKey.D:
                                _peer.Leave();
                                continue;
                        }
                    }

                    sb.Append(ch.KeyChar);
                }
            }
            */
        }

        private bool PreProcess(string input) {
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

        /// <summary>
        ///     First, we translate the input (which could be any supported language) to Pi.
        ///     Then we convert that to Pi text and send to current server.
        /// </summary>
        private bool Execute(string input) {
            if (string.IsNullOrEmpty(input)) {
                // get refresh of remote data stack
                _peer.Remote?.Continue(" ");
                return true;
            }

            try {
                if (PreProcess(input)) {
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

        private static void OnConnected(IPeer peer, IClient client) {
            WriteLine($"Connected to {client}.");
        }

        private static void WriteDataStackContents(INetCommon client, int max = 50) {
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

        private bool ShowHelp() {
            SystemConsole.WriteLine(
                @"
The prompt shows the current executing context and language.
When you type at the prompt, your text is executed in the current context - which could be local (default) or remote.
Before the prompt is printed, the data-stack of the current server is printed. Operations you perform act on this data-stack. Each connection to a remote server has its own private context.
To connect to a remote node, type:

Rho> peer.Connect(""_hostName"", port) 

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