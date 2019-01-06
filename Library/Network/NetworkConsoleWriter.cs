using System;
using Con = System.Console;

namespace Diver.Network
{
    public class NetworkConsoleWriter
    {
        protected static bool Error(string text)
        {
            WriteLine($"Error: {text}");
            return false;
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
