using System.Collections.Generic;
using Diver.Impl;
using NUnit.Framework;

namespace Diver.Test
{
    /// <summary>
    /// Common to all (most) unit tests in the system
    /// </summary>
    [TestFixture]
    public class TestCommon
    {
        protected const string ScriptsFolder = "Scripts";

        [SetUp]
        public void Setup()
        {
            _reg = new Registry();
            _executor = _reg.Add(new Exec.Executor());
            _exec = _executor.Value;
        }

        protected object Pop()
        {
            Assert.Greater(DataStack.Count, 0);
            return DataStack.Pop();
        }

        protected T Pop<T>()
        {
            var top = Pop();
            if (top is T result)            // deal with unwrapped values
                return result;
            var typed = top as IRef<T>;     // deal with boxed values
            Assert.IsNotNull(typed);
            return typed.Value;
        }

        protected void WriteLine(string fmt, params object[] args)
        {
            var text = string.Format(fmt, args);
            System.Diagnostics.Trace.WriteLine(text);
            TestContext.Out.WriteLine(text);
            System.Console.WriteLine(text);
        }

        protected Stack<object> DataStack => _exec.DataStack;
        protected IRegistry _reg;
        protected IRef<Diver.Exec.Executor> _executor;
        protected Diver.Exec.Executor _exec;
    }
}
