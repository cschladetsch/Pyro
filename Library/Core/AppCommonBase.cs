using System.Runtime.InteropServices;

namespace Pyro
{
    using System;
    using System.Reflection;
    using static System.Console;
    using Con = System.Console;

    /// <summary>
    /// Functionality that is common to all (console) Apps that use Pyro libraries.
    /// </summary>
    public abstract class AppCommonBase
    {
        private static AppCommonBase _self;
        private readonly ConsoleColor _originalColor;

        protected AppCommonBase(string[] args)
        {
            CancelKeyPress += Cancel;
            _originalColor = ForegroundColor;
            _self = this;
            WriteHeader();
        }

        public static string GetVersion()
        {
            var asm = Assembly.GetEntryAssembly();
            var desc = asm.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            var name = asm.GetName();
            var version = name.Version;

            var built = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.MinorRevision*2);
            var b = built.ToString("yy-MM-ddTHH:mm");
            var v = $"{version.Build}.{version.Revision}";

            return $"{desc} v{v} built {b}";
        }

        protected virtual void ProcessArgs(string[] args)
        {
        }

        protected abstract void Shutdown();

        protected void Exit(int result = 0)
        {
            ForegroundColor = _originalColor;
            Environment.Exit(result);
        }

        protected static bool Error(string text, ConsoleColor color = ConsoleColor.Green)
        {
            WriteLine(text, color);
            return false;
        }

        protected static void Write(string text, ConsoleColor color = ConsoleColor.White)
            => ConWrite(text, color, Con.Write);

        protected static void WriteLine(string text, ConsoleColor color = ConsoleColor.White)
            => ConWrite(text, color, Con.WriteLine);

        private static void WriteHeader()
            => WriteLine($"{GetVersion()}", ConsoleColor.DarkGray);

        /// <summary>
        /// Save/restore current foreground color while writing a string to the console.
        /// </summary>
        private static void ConWrite(string text, ConsoleColor color, Action<string> write)
        {
            var current = ForegroundColor;
            ForegroundColor = color;
            write(text);
            ForegroundColor = current;
        }

        private static void Cancel(object sender, ConsoleCancelEventArgs e)
        {
            // don't exit immediately - shut down networking gracefully first
            e.Cancel = true;
            _self.Shutdown();
        }
    }
}

