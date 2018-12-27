using System;
using System.Text;

namespace Diver.Exec
{
    /// <summary>
    /// Debug methods for executor. Removed from main implementation for clarity.
    /// </summary>
    public partial class Executor
    {
        private string DebugWrite()
        {
            var str = new StringBuilder();
            if (_data == null)
            {
                str.AppendLine("No Data");
            }
            else
            {
                WriteDataStack(str);
            }
            if (Context() == null)
            {
                str.AppendLine("No continuation");
            }
            else
            {
                str.AppendLine("Context:");
                Context().DebugWrite(str);
            }
            return str.ToString();
        }

        private void WriteDataStack(StringBuilder str, int max = 4)
        {
            str.AppendLine("Data:");
            var data = _data.ToArray();
            max = Math.Min(data.Length, max);
            for (int n = max - 1; n >= 0; --n)
            {
                str.AppendLine($"    [{n}]: {data[n]}");
            }
        }
    }
}
