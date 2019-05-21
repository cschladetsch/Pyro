using System;

namespace Pyro
{
    /// <summary>
    /// Common to all processes.
    /// </summary>
    public class ProcessCommon
        : Process
    {
        protected ProcessCommon(IRegistry r)
        {
            _reg = r;
        }

        protected IRef<T> New<T>()
        {
            return _reg.Add<T>(default);
        }

        protected IRef<T> New<T>(T val)
        {
            return _reg.Add(val);
        }

        protected void WriteLine(object obj)
        {
            WriteLine("{0}", obj.ToString());
        }

        protected void WriteLine(string fmt, params object[] args)
        {
            string text = fmt;
            if (args != null && args.Length > 0)
                text = string.Format(fmt, args);
            System.Diagnostics.Trace.WriteLine(text);
            Console.WriteLine(text);
        }

        protected readonly IRegistry _reg;

        protected new void Reset()
        {
            base.Reset();
        }
    }
}