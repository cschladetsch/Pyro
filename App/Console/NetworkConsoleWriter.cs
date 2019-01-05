using System;
using Con = System.Console;

namespace Console
{
    public class NetworkConsoleWriter
    {
        protected static void Error(string text)
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
