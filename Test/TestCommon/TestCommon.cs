using System;
using System.Collections.Generic;
using System.IO;
using Diver.Impl;
using Diver.Language;
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
        public bool Verbose = true;

        [SetUp]
        public void Setup()
        {
            _reg = new Registry();
            _executor = _reg.Add(new Exec.Executor());
            _exec = _executor.Value;
        }

        protected string GetFullScriptPathname(string scriptName)
        {
            return Path.Combine(GetScriptsPath(), scriptName);
        }

        protected bool RunScript(string scriptName)
        {
            return RunScriptPathname(GetFullScriptPathname(scriptName));
        }

        // TODO: overkill at the moment
        //protected ITranslator MakeTranslator(string scriptName)
        //{
        //    //new PiTranslator(_reg, text)t;
        //    object klass = null;
        //    switch (Path.GetExtension(scriptName))
        //    {
        //        case ".pi":
        //            klass = typeof(PiTranslator);
        //            break;
        //        case ".rho":
        //            klass = typeof(RhoTranslator);
        //            break;
        //        case ".tau":
        //            klass = typeof(TauTranslator);
        //            break;
        //    }
        //    return Activator.CreateInstance(type, _reg, text)
        //}

        protected bool RunScriptPathname(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            try
            {
                WriteLine($"Running {fileName}");
                var text = File.ReadAllText(filePath);
                var trans = new PiTranslator(_reg, text);
                if (Verbose)
                    WriteLine($"Translator for {filePath}: {trans}");
                if (trans.Failed)
                    WriteLine($"Error: {trans.Error}");
                Assert.IsFalse(trans.Failed);
                _exec.Continue(trans.Continuation);
            }
            catch (Exception e)
            {
                WriteLine($"Script {fileName}: Exception={e.Message}");
                return false;
            }

            return true;
        }
        protected string GetScriptsPath()
        {
            var root = TestContext.CurrentContext.TestDirectory.Replace(@"\bin\Debug", "");
            return Path.Combine(root, ScriptsFolder);
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

        protected void WriteLine(object obj)
        {
            WriteLine("{0}", obj.ToString());
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
