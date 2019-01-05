using System;
using System.Text;
using Diver;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;

using Con = System.Console;

namespace Console
{
    class Program
    {
        private Program(string[] args)
        {
            _registry = new Registry();
            _exec = new Executor(_registry);
            _piTranslator = new PiTranslator(_registry);
            _rhoTranslator = new RhoTranslator(_registry);
        }

        static void Main(string[] args)
        {
            new Program(args).Repl();
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
                    Error($"Exception: {e.Message})");
                }
            }
        }

        private void Process()
        {
            WritePrompt();
            var input = Con.ReadLine();
            if (!_piTranslator.Translate(input))
            {
                Error($"Error: {_piTranslator.Error}");
                return;
            }

            _exec.Continue(_piTranslator.Result());
            WriteDataStack();
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
            var str = new StringBuilder();
            Con.ForegroundColor = ConsoleColor.Yellow;
            _exec.WriteDataStack(str, 20);
            Con.WriteLine(str.ToString());
        }

        private string MakePrompt()
        {
            return "> ";
        }

        private readonly IRegistry _registry;
        private readonly Executor _exec;
        private readonly PiTranslator _piTranslator;
        private RhoTranslator _rhoTranslator;
    }
}
