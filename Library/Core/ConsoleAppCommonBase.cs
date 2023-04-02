using System;
using System.Reflection;

namespace Pyro.AppCommon {
    using static Console;
    using Con = Console;

    public abstract class ConsoleAppCommonBase {
        private static ConsoleAppCommonBase _self;
        private readonly ConsoleColor _originalColor;
        private readonly object _consoleLock = new object();

        protected ConsoleAppCommonBase(string[] args) {
            CancelKeyPress += Cancel;
            _originalColor = ForegroundColor;
            _self = this;
            WriteHeader();
        }

        public static string GetVersion() {
            var asm = Assembly.GetEntryAssembly();
            if (asm == null) {
                return "[Pyro]";
            }
            var desc = asm.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            var name = asm.GetName();
            var version = name.Version;
            var built = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.MinorRevision * 2);

            return $"{desc} v{version.Build}.{version.Revision} built {built:yy-MM-ddTHH:mm}";
        }

        protected abstract void Shutdown();

        protected void Exit(int result = 0) {
            ForegroundColor = _originalColor;
            Environment.Exit(result);
        }

        protected static bool Error(string text, ConsoleColor color = ConsoleColor.Red) {
            WriteLine(text, color);
            return false;
        }

        protected void Write(string text, ConsoleColor color = ConsoleColor.White) {
            ConWrite(text, color, Con.Write);
        }

        protected static void WriteLine(string text, ConsoleColor color = ConsoleColor.White) {
            ConWrite(text, color, Con.WriteLine);
        }

        private static void WriteHeader() {
            WriteLine($"{GetVersion()}", ConsoleColor.DarkGray);
        }

        private static void ConWrite(string text, ConsoleColor color, Action<string> write) {
            lock (_self._consoleLock) {
                var current = ForegroundColor;
                ForegroundColor = color;
                write(text);
                ForegroundColor = current;
            }
        }

        private static void Cancel(object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            _self.Shutdown();
        }
    }
}
