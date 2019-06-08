using System;

namespace Pyro
{
    public static class Timer
    {
        public static string Time(string label, Action action)
        {
            var start = DateTime.Now;
            action();
            var milliseconds = (DateTime.Now - start).TotalMilliseconds;
            return $"{label} took {milliseconds}ms";
        }
    }
}
