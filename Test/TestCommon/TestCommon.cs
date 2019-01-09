using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;
using Diver.Language.Impl;
using NUnit.Framework;

namespace Diver.Test
{
    /// <summary>
    /// Common to most unit tests in the system
    /// </summary>
    [TestFixture]
    public class TestCommon
    {
        public bool Verbose = true;
        public const string ScriptsFolder = "Scripts";

        protected Continuation _continuation;
        protected IDictionary<string, object> _scope => _continuation?.Scope;
        protected IList<object> _code => _continuation?.Code;
        protected Stack<object> DataStack => _exec.DataStack;
        protected IRegistry _reg;
        protected IRef<Executor> _executor;
        protected Executor _exec => _executor.Value;
        private ITranslator _pi;
        private ITranslator _rho;

        [SetUp]
        public void Setup()
        {
            _reg = new Registry();
            _pi = new PiTranslator(_reg);
            _rho = new PiTranslator(_reg);
            _executor = _reg.Add(new Executor());
            Exec.RegisterTypes.Register(_reg);
        }

        protected void PiRun(string text)
        {
            _exec.Clear();
            _exec.Continue(_continuation = PiTranslate(text));
        }

        protected void RhoRun(string text, bool trace = false, EStructure st = EStructure.Program)
        {
            _exec.Clear();
            Time("Exec took ", () => _exec.Continue(_continuation = RhoTranslate(text, trace, st)));
        }

        protected void Time(string label, Action action)
        {
            WriteLine(Timer.Time("\t" + label, action));
        }

        protected Continuation PiTranslate(string text)
        {
            var trans = new PiTranslator(_reg);
            trans.Translate(text);
            if (trans.Failed)
                WriteLine($"Error: {trans.Error}");
            Assert.IsFalse(trans.Failed);
            return _continuation = trans.Result;
        }

        protected Continuation RhoTranslate(string text, bool trace = false, EStructure st = EStructure.Program)
        {
            var trans = new RhoTranslator(_reg);
            trans.Translate(text, st);

            if (trans.Result == null)
                WriteLine($"No output generated");
            if (trans.Failed)
                WriteLine($"Error: {trans.Error}");
            if (trace)
                WriteLine(trans.ToString());
            Assert.IsFalse(trans.Failed);
            return _continuation = trans.Result;
        }

        protected bool RunScript(string scriptName)
        {
            return RunScriptPathname(GetFullScriptPathname(scriptName));
        }

        protected string GetFullScriptPathname(string scriptName)
        {
            return Path.Combine(GetScriptsPath(), scriptName);
        }

        protected ITranslator MakeTranslator(string scriptName)
        {
            Type klass = null;
            switch (Path.GetExtension(scriptName))
            {
                case ".pi":
                    klass = typeof(PiTranslator);
                    break;
                case ".rho":
                    klass = typeof(RhoTranslator);
                    break;
                //case ".tau":
                //    klass = typeof(TauTranslator);
                //    break;
            }

            return Activator.CreateInstance(klass, _reg) as ITranslator;
        }

        protected string LoadScript(string fileName)
        {
            return File.ReadAllText(GetFullScriptPathname(fileName));
        }

        protected Continuation TranslateScript(string fileName)
        {
            var trans = MakeTranslator(fileName);
            Assert.IsTrue(trans.Translate(LoadScript(fileName)));
            return trans.Result;
        }

        protected bool RunScriptPathname(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            try
            {
                WriteLine($"Running {fileName}");
                _exec.SourceFilename = fileName;
                var text = File.ReadAllText(filePath);
                var trans = MakeTranslator(filePath);
                trans.Translate(text);
                if (trans.Failed)
                    WriteLine($"Error: {trans.Error}");
                Assert.IsFalse(trans.Failed);
                Time($"Exec script `{fileName}`", () => _exec.Continue(trans.Result));
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

        protected void AssertEmpty()
        {
            Assert.AreEqual(0, DataStack.Count);
        }

        protected void AssertPop<T>(T val)
        {
            Assert.AreEqual(val, Pop<T>());
        }

        protected object Pop()
        {
            Assert.Greater(DataStack.Count, 0);
            return DataStack.Pop();
        }

        protected T Pop<T>()
        {
            var top = Pop();
            if (top is T result) // deal with unwrapped values
                return result;
            var typed = top as IConstRef<T>; // deal with boxed values
            Assert.IsNotNull(typed);
            return typed.Value;
        }

        protected void WriteLine(object obj)
        {
            WriteLine($"{obj}");
        }

        protected void WriteLine(string fmt, params object[] args)
        {
            var text = fmt;
            if (args != null && args.Length > 0)
                text = string.Format(fmt, args);
            TestContext.Out.WriteLine(text);
            //System.Diagnostics.Trace.WriteLine(text);
            //Console.WriteLine(text);
        }

        protected PiLexer PiLex(string input)
        {
            var lex = new PiLexer(input);
            if (lex.Failed)
                WriteLine("LexerFailed: {0}", lex.Error);
            Assert.IsTrue(lex.Process());
            return lex;
        }

        protected T ConstRef<T>(object o)
        {
            return Executor.ConstRef<T>(o);
        }

        protected void AssertVarEquals<T>(string ident, T val)
        {
            Assert.IsTrue(_scope.ContainsKey(ident));
            var obj = _scope[ident];
            switch (obj)
            {
                case T v:
                    Assert.AreEqual(v, val);
                    return;
                case IRefBase rb:
                    Assert.AreEqual(rb.BaseValue, val);
                    return;
            }
        }

        protected void AssertSameTokens(string input, params EPiToken[] tokens)
        {
            var lex = PiLex(input);
            AssertSameTokens(lex.Tokens, tokens);
        }

        protected void AssertSameTokens(IEnumerable<object> input, params EPiToken[] tokens)
        {
            var piTokens = input.Cast<PiToken>().Where(t => !IsWhiteSpace(t)).Select(t => t.Type).ToList();
            var expected = tokens.ToList();
            Assert.AreEqual(piTokens.Count, expected.Count);
            Assert.IsTrue(piTokens.SequenceEqual(expected));
        }

        protected bool IsWhiteSpace(PiToken piToken)
        {
            switch (piToken.Type)
            {
                case EPiToken.Whitespace:
                case EPiToken.Tab:
                case EPiToken.NewLine:
                    return true;
            }

            return false;
        }

        protected void TestScript(string scriptName)
        {
            Assert.IsTrue(RunScript(scriptName), $"Script={scriptName}");
            Assert.AreEqual(0, _exec.DataStack.Count);
        }

        protected void TestFreezeThawScript(string fileName)
        {
            var text = TranslateScript(fileName).ToText();
            WriteLine(text);
            TestFreezeThawPi(text);
        }

        protected void TestFreezeThawPi(string text)
        {
            Assert.IsTrue(Continue(FreezeThaw(_pi, text)));            
        }

        protected void TestFreezeThawRho(string text)
        {
            Assert.IsTrue(Continue(FreezeThaw(_rho, text)));            
        }

        protected bool Continue(Continuation cont)
        {
            Assert.IsNotNull(cont);
            try
            {
                _exec.Continue(cont);
                return true;
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
                return false;
            }
        }

        protected Continuation FreezeThaw(ITranslator trans, string text)
        {
            WriteLine("--- Input:");
            WriteLine(text);
            var cont = PiTranslate(text);

            WriteLine("--- Serialised:");
            var str = cont.ToText();
            WriteLine(str);
            Assert.IsNotEmpty(str);

            var thawed = PiTranslate(str);
            Assert.IsNotNull(thawed);
            var continuation = thawed.Code[0] as Continuation;
            Assert.IsNotNull(continuation);
            return continuation;
        }
    }
}
