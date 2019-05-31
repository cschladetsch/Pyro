using System;
using System.Text;

namespace Pyro.Exec
{
    /// <inheritdoc />
    /// <summary>
    /// Debug methods for executor. Removed from main implementation for clarity.
    /// </summary>
    public partial class Executor
    {
        public int TraceLevel;

        private void Write(object obj)
        {
            Write($"{obj}");
        }

        private void WriteLine(object obj)
        {
            WriteLine($"{obj}");
        }

        static void Write(string text, params object[] args)
        {
            System.Diagnostics.Debug.Write(text);
            Console.Write(text);
        }

        private void WriteLine(string fmt, params object[] args)
        {
            if (args == null || args.Length == 0)
                Write(fmt + '\n');
            else
                Write($"{string.Format(fmt, args)}\n");
        }

        private void DebugTrace()
        {
            WriteLine(DebugWrite());
        }

        private string DebugWrite()
        {
            var str = new StringBuilder();
            WriteDataStack(str);
            WriteContinuation(str);
            return str.ToString();
        }

        private void WriteContinuation()
        {
            var str = new StringBuilder();
            WriteContinuation(str);
            WriteLine(str);
        }

        private void WriteContinuation(StringBuilder str)
        {
            str.AppendLine("Context:");
            if (Context() == null)
                str.AppendLine("    No continuation");
            else
                Context().DebugWrite(str);
        }

        public void WriteDataStack(int max = 4)
        {
            var str = new StringBuilder();
            WriteDataStack(str, max);
            WriteLine(str);
        }

        public void WriteDataStack(StringBuilder str, int max = 4)
        {
            str.AppendLine("Data:");
            var data = DataStack.ToArray();
            max = Math.Min(data.Length, max);
            for (var n = max - 1; n >= 0; --n)
            {
                var obj = data[n];
                str.AppendLine($"    [{n}]: {GetTyped(obj)}");
            }
        }

        private static string GetTyped(object obj)
        {
            return obj == null ? "null" : $"{obj} ({obj.GetType().Name})";
        }

        private void PerformPrelude(object next)
        {
            if (TraceLevel == 0)
                return;
            var str = new StringBuilder();
            str.Append("========\n");
            if (TraceLevel > 5)
            {
                WriteDataStack(str);
                str.AppendLine($"next: '{GetTyped(next)}'");
            }
            WriteLine(str);
        }
    }
}

