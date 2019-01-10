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

        protected new bool Error(string text)
        {
            Fail(text);
            WriteLine($"Error: {text}");
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
