using System.Diagnostics;
using NUnit.Framework;

namespace Pyro.Exec
{
    using System;
    using System.Text;

    /// <inheritdoc />
    /// <summary>
    /// Debug methods for executor. Removed from main implementation for clarity.
    /// </summary>
    public partial class Executor
    {
        private static void Write(object obj)
            => Write($"{obj}");

        private static void WriteLine(object obj)
            => WriteLine($"{obj}");

        private static void Write(string text, params object[] args)
        {
            System.Diagnostics.Debug.Write(text);
            //Console.Write(text, args);
            //Debug.Write(text);
            Console.Write(text);
            Trace.Write(text);
            TestContext.Out.WriteLine(text);
        }

        private static void WriteLine(string fmt, params object[] args)
        {
            if (args == null || args.Length == 0)
                Write(fmt + '\n');
            else
                Write($"{string.Format(fmt, args)}\n");
        }

        public void DebugTrace()
            => WriteLine(DebugWrite());

        private string DebugWrite()
        {
            var str = new StringBuilder();
            WriteDataStack(str);
            WriteContextStack(str);
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
                str.AppendLine($"\t[{n}]: {GetTyped(obj)}");
            }
        }

        public void WriteContextStack(StringBuilder str, int max = 4)
        {
            str.AppendLine("Context:");
            max = Math.Min(ContextStack.Count, max);
            if (max == 0)
            {
                str.AppendLine("\tEmpty");
                return;
            }
            for (var n = max - 1; n >= 0; --n)
            {
                var obj = ContextStack[n];
                str.Append($"\t[{n}]: ");
                obj.DebugWrite(str);
                str.AppendLine();
            }
        }

        private static string GetTyped(object obj)
            => obj == null ? "null" : $"{obj} ({obj.GetType().Name})";

        private void PerformPrelude(object next)
        {
            if (Verbosity == 0)
                return;

            var str = new StringBuilder();
            str.AppendLine($"---- Step #{Kernel.StepNumber}");

            if (Verbosity > 5)
            {
                WriteDataStack(str);
                str.AppendLine("Current: ");
                _current?.DebugWrite(str);
                str.AppendLine();
                WriteContextStack(str);
                str.AppendLine($"Next:\n\t'{GetTyped(next)}'");
            }

            WriteLine(str.ToString());
        }
    }
}

