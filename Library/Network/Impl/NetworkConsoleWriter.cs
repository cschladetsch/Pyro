using System;
using Con = System.Console;

using Flow;

namespace Pyro.Network.Impl {

    public class NetworkConsoleWriter
        : Process {
        public event OnWriteDelegate OnWrite;

        public override bool Fail(string text) {
            OnWrite?.Invoke(ELogLevel.Error, text);
            base.Fail(text);
            Error(text);
            return false;
        }

        protected bool Warn(string text) {
            OnWrite?.Invoke(ELogLevel.Warn, text);
            WriteLine($"Warn: {text}", ConsoleColor.Yellow);
            return true;
        }

        protected new bool Error(string text) {
            OnWrite?.Invoke(ELogLevel.Error, text);
            WriteLine($"Error: {text}", ConsoleColor.Red);
            base.Error = text;
            return false;
        }

        protected bool WriteLine(string text, ConsoleColor color = ConsoleColor.White) {
            OnWrite?.Invoke(ELogLevel.Info, text);

            var previousColor = Con.ForegroundColor;
            Con.ForegroundColor = color;
            Con.WriteLine(text);
            Con.ForegroundColor = previousColor;
            return true;
        }
    }
}