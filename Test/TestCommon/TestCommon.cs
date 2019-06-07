using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace Pyro.Test
{
    using Exec;
    using Impl;
    using Language;
    using Language.Lexer;
    using RhoLang;

    /// <inheritdoc />
    /// <summary>
    /// Common to most unit tests in the system
    /// </summary>
    [TestFixture]
    public class TestCommon
        : Process
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
            _rho = new RhoTranslator(_reg);
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

        protected Continuation PiTranslate(string text)
        {
            var trans = new PiTranslator(_reg);
            if (!trans.Translate(text, out var cont))
                WriteLine($"Error: {trans.Error}");

            Assert.IsFalse(trans.Failed, trans.Error);
            return _continuation = cont;
        }

        protected Continuation RhoTranslate(string text, bool trace = false, EStructure st = EStructure.Program)
        {
            var trans = new RhoTranslator(_reg);
            if (!trans.Translate(text, out var cont, st))
                WriteLine($"Error: {trans.Error}");

            if (trace)
                WriteLine(trans.ToString());

            Assert.IsFalse(trans.Failed, trans.Error);
            return _continuation = cont;
        }

        protected ITranslator MakeTranslator(string scriptName)
        {
            Type klass = null;
            var extension = Path.GetExtension(scriptName);
            switch (extension)
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
                default:
                    throw new NotImplementedException($"Unsupported script {extension}");
            }

            return Activator.CreateInstance(klass, _reg) as ITranslator;
        }

        protected Continuation TranslateScript(string fileName)
        {
            var trans = MakeTranslator(fileName);
            Assert.IsTrue(trans.Translate(LoadScript(fileName), out var cont));
            return cont;
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
                if (!trans.Translate(text, out var cont))
                    WriteLine($"Error: {trans.Error}");

                Assert.IsFalse(trans.Failed);
                Time($"Exec script `{fileName}`", () => _exec.Continue(cont));
            }
            catch (Exception e)
            {
                WriteLine($"Script {fileName}: Exception={e.Message}");
                return false;
            }

            return true;
        }

        protected void Time(string label, Action action) 
            => WriteLine(Timer.Time("\t" + label, action));

        protected string LoadScript(string fileName)
            => File.ReadAllText(GetFullScriptPathname(fileName));

        protected bool RunScript(string scriptName)
            => RunScriptPathname(GetFullScriptPathname(scriptName));

        protected string GetFullScriptPathname(string scriptName)
            => Path.Combine(GetScriptsPath(), scriptName);

        protected string GetScriptsPath()
            => MakeLocalPath(ScriptsFolder);

        protected string MakeLocalPath(string relative)
            => Path.Combine(GetFolderRoot(), relative);

        private static string GetFolderRoot()
            => TestContext.CurrentContext.TestDirectory.Replace(@"\bin\Debug", "");

        protected void AssertEmpty()
            => Assert.AreEqual(0, DataStack.Count);

        protected void AssertPop<T>(T val)
            => Assert.AreEqual(val, Pop<T>());

        protected T ConstRef<T>(object o)
            => Executor.ConstRef<T>(o);

        protected void TestFreezeThawPi(string text)
            => Assert.IsTrue(Continue(FreezeThaw(_pi, text)));

        protected void TestFreezeThawRho(string text)
            => Assert.IsTrue(Continue(FreezeThaw(_rho, text)));

        protected object Pop()
        {
            Assert.Greater(DataStack.Count, 0, "Empty Datastack");
            return DataStack.Pop();
        }

        protected T Pop<T>()
        {
            var top = Pop();
            if (top is T result)                // Deal with unwrapped values.
                return result;

            var typed = top as IConstRef<T>;    // Deal with boxed values.
            Assert.IsNotNull(typed);
            return typed.Value;
        }

        protected void WriteLine(string fmt, params object[] args)
        {
            var text = fmt;
            if (args != null && args.Length > 0)
                text = string.Format(fmt, args);

            DebugTraceLine(text);
        }

        private static void DebugTraceLine(string text)
        {
            TestContext.Out.WriteLine(text);
            //System.Diagnostics.Trace.WriteLine(text);
            //Console.WriteLine(text);
        }

        protected PiLexer PiLex(string input)
        {
            var lex = new PiLexer(input);
            if (lex.Failed)
                WriteLine("LexerFailed: {0}", lex.Error);

            Assert.IsTrue(lex.Process(), lex.Error);
            return lex;
        }

        protected void AssertVarEquals<T>(string ident, T val)
        {
            Assert.IsTrue(_scope.ContainsKey(ident), $"{ident} not found");
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

        protected void FreezeThaw(string fileName)
        {
            var script = LoadScript(fileName);
            switch (Path.GetExtension(fileName))
            {
                case ".pi":
                    TestFreezeThawPi(script);
                    return;

                case ".rho":
                    TestFreezeThawRho(script);
                    return;
            }

            Assert.Fail($"Unsupported extension {fileName}");
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
            Assert.IsTrue(trans.Translate(text, out var cont));

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

