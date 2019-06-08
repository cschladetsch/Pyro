using System;
using System.Reflection;

using Con = System.Console;

namespace Pyro.AppCommon
{
    /// <summary>
    /// Functionality that is common to all Apps that use Pyro libraries.
    /// </summary>
    public abstract class AppCommonBase
    {
        private static AppCommonBase _self;
        private readonly ConsoleColor _originalColor;

        protected AppCommonBase(string[] args)
        {
            Con.CancelKeyPress += Cancel;
            _originalColor = Con.ForegroundColor;
            _self = this;
            WriteHeader();
        }

        protected virtual void ProcessArgs(string[] args)
        {
        }

        protected abstract void Shutdown();

        private static void WriteHeader()
        {
            WriteLine($"{GetVersion()}", ConsoleColor.DarkGray);
        }

        public static string GetVersion()
        {
            var name = Assembly.GetExecutingAssembly().GetName();
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
            WriteLine(text, color);
            return false;
        }

        protected static void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            ConWrite(text, color, Con.Write);
        }

        protected static void WriteLine(string text, ConsoleColor color = ConsoleColor.White)
        {
            ConWrite(text, color, Con.WriteLine);
        }

        /// <summary>
        /// Save/restore current foreground color while writing a string to the console.
        /// </summary>
        private static void ConWrite(string text, ConsoleColor color, Action<string> write)
        {
            //System.Console.WriteLine(text);
            var current = Con.ForegroundColor;
            Con.ForegroundColor = color;
            write(text);
            Con.ForegroundColor = current;
        }

        private static void Cancel(object sender, ConsoleCancelEventArgs e)
        {
            // don't exit immediately - shut down networking gracefully first
            e.Cancel = true;
            _self.Shutdown();
        }
    }
}

