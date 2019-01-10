using System;
using Con = System.Console;

namespace Diver.Network
{
    public class NetworkConsoleWriter : Process
    {
        public override bool Fail(string text)
        {
            base.Fail(text);
            Error(text);
            return false;
        }

        protected new static void Error(string text)
        {
            WriteLine($"Error: {text}");
        }

        protected static void WriteLine(string text)
        {
            var color = Con.ForegroundColor;
            Con.ForegroundColor = ConsoleColor.Cyan;
            Con.WriteLine();
            Con.WriteLine(text);
            Con.ForegroundColor = color;
        }
    }
}
