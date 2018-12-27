using System;
using System.Text;

namespace Diver.Exec
{
    /// <summary>
    /// Debug methods for executor. Removed from main implementation for clarity.
    /// </summary>
    public partial class Executor
    {
        public int TraceLevel;

        void Write(object obj)
        {
            Write($"{obj}");
        }

        void WriteLine(object obj)
        {
            WriteLine($"{obj}");
        }

        void Write(string text, params object[] args)
        {
            System.Diagnostics.Debug.Write(text);
            Console.Write(text);
        }
        
        void WriteLine(string fmt, params object[] args)
        {
            Write($"{string.Format(fmt, args)}\n");
        }

        private string DebugWrite()
        {
            var str = new StringBuilder();
            WriteDataStack(str);
            WriteContinuation(str);
            return str.ToString();
        }

        private void WriteContinuation(StringBuilder str)
        {
            str.AppendLine("Context:");
            if (Context() == null)
                str.AppendLine("    No continuation");
            else
                Context().DebugWrite(str);
        }

        private void WriteDataStack(int max = 4)
        {
            var str = new StringBuilder();
            WriteDataStack(str, max);
            WriteLine(str);
        }

        private void WriteDataStack(StringBuilder str, int max = 4)
        {
            str.AppendLine("Data:");
            var data = _data.ToArray();
            max = Math.Min(data.Length, max);
            for (int n = max - 1; n >= 0; --n)
            {
                var obj = data[n];
                var type = obj.GetType().FullName;
                if (type.StartsWith("System."))
                    type = type.Remove(0, 7);
                str.AppendLine($"    [{n}]: {obj} ({type})");
            }
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
                str.AppendLine($"next: '{next}'");
            }
            WriteLine(str);
        }
    }
}
