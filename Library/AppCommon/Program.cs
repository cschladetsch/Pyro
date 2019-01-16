using System;
using System.Reflection;
using Con = System.Console;

namespace Pyro.AppCommon
{
    public abstract class AppCommonBase
    {
        protected AppCommonBase(string[] args)
        {
            Con.CancelKeyPress += Cancel;
            _originalColor = Con.ForegroundColor;
            _self = this;
            WriteHeader();
        }

        private void WriteHeader()
        {
            Write($"{GetVersion()}\n", ConsoleColor.DarkGray);
        }

        private static string GetVersion()
        {
            var name = Assembly.GetEntryAssembly().GetName();
            var version = name.Version;
            var built = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.MinorRevision * 2);
            return $"Pyro {name.Name} {version} built {built}";
        }

        protected void Exit(int result = 0)
        {
            Con.ForegroundColor = _originalColor;
            Environment.Exit(result);
        }

        protected static bool Error(string text, ConsoleColor color = ConsoleColor.Green)
        {
            Write(text, color);
            return false;
        }

        protected static void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            var current = Con.ForegroundColor;
            Con.ForegroundColor = color;
            Con.Write(text);
            Con.ForegroundColor = current;
        }

        protected static void WriteLine(string text, ConsoleColor color = ConsoleColor.White)
        {
            var current = Con.ForegroundColor;
            Con.ForegroundColor = color;
            Con.WriteLine(text);
            Con.ForegroundColor = current;
        }
        
        private static void Cancel(object sender, ConsoleCancelEventArgs e)
        {
            // don't exit immediately - shut down networking gracefully first
            e.Cancel = true;
            _self.Shutdown();
        }

        protected abstract void Shutdown();

        private static AppCommonBase _self;
        private readonly ConsoleColor _originalColor;
    }
}
