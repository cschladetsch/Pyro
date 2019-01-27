using System;
using Pryo;
using Con = System.Console;

namespace Pyro.Network.Impl
{
    public class NetworkConsoleWriter : Process
    {
        public override bool Fail(string text)
        {
            base.Fail(text);
            Error(text);
            return false;
        }

        protected new bool Error(string text)
        {
            WriteLine($"Error: {text}");
            base.Error = text;
            return false;
        }

        protected static bool WriteLine(string text)
        {
            var color = Con.ForegroundColor;
            Con.ForegroundColor = ConsoleColor.Cyan;
            Con.WriteLine();
            Con.WriteLine(text);
            Con.ForegroundColor = color;
            return true;
        }
    }
}
