﻿using System;
using System.Diagnostics;

namespace Pyro {
    /// <inheritdoc />
    public class ProcessCommon : Process {
        protected readonly IRegistry _reg;

        protected ProcessCommon(IRegistry r) {
            _reg = r;
        }

        protected IRef<T> New<T>() {
            return _reg.Add<T>(default);
        }

        protected IRef<T> New<T>(T val) {
            return _reg.Add(val);
        }

        protected void WriteLine(object obj) {
            WriteLine("{0}", obj.ToString());
        }

        protected new void Reset() {
            base.Reset();
        }

        protected void WriteLine(string fmt, params object[] args) {
            var text = fmt;
            if (args != null && args.Length > 0) {
                text = string.Format(fmt, args);
            }

            Trace.WriteLine(text);
            Console.WriteLine(text);
        }
    }
}