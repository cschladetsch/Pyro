using System.Text;

namespace Pyro.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Exec;
    using Impl;
    using Language;
    using Language.Lexer;
    using RhoLang;

    /// <inheritdoc />
    /// <summary>
    /// Common to most unit tests in the system.
    /// </summary>
    [TestFixture]
    public class TestCommon
        : Process
    {
        public bool Verbose = true;
        public const string ScriptsFolder = "Scripts";

        protected Continuation _Continuation;
        protected IDictionary<string, object> _Scope => _Continuation?.Scope;
        protected IList<object> _Code => _Continuation?.Code;
        protected Stack<object> DataStack => _Exec.DataStack;
        protected IRegistry _Registry;
        protected IRef<Executor> _ExecutorRef;
        protected Executor _Exec => _ExecutorRef.Value;

        private ITranslator _pi;
        private ITranslator _rho;

        [SetUp]
        public void Setup()
        {
            _Registry = new Registry();
            Exec.RegisterTypes.Register(_Registry);

            _pi = new PiTranslator(_Registry);
            _rho = new RhoTranslator(_Registry);
            _ExecutorRef = _Registry.Add(new Executor());
        }

        protected void PiRun(string text)
        {
            _Exec.Clear();
            _Exec.Continue(_Continuation = PiTranslate(text));
        }

        protected void RhoRun(string text, bool trace = false, EStructure st = EStructure.Program)
        {
            _Exec.Clear();
            _Exec.Continue(_Continuation = RhoTranslate(text, trace, st));
        }

        protected Continuation PiTranslate(string piScript)
        {
            var trans = new PiTranslator(_Registry);
            if (!trans.Translate(piScript, out var cont))
                WriteLine($"Error: {trans.Error}");

            Assert.IsFalse(trans.Failed, trans.Error);
            return _Continuation = cont;
        }

        protected Continuation RhoTranslate(string rhoScript, bool trace = false, EStructure st = EStructure.Program)
        {
            var trans = new RhoTranslator(_Registry);
            if (!trans.Translate(rhoScript, out var cont, st))
                WriteLine($"Error: {trans.Error}");

            if (trace)
                WriteLine(trans.ToString());

            Assert.IsFalse(trans.Failed, trans.Error);
            return _Continuation = cont;
        }

        private ITranslator MakeTranslator(string scriptName)
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
                default:
                    throw new NotImplementedException($"Unsupported script {extension}");
            }

            return Activator.CreateInstance(klass, _Registry) as ITranslator;
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
                _Exec.SourceFilename = fileName;
                var text = File.ReadAllText(filePath);
                var trans = MakeTranslator(filePath);
                if (!trans.Translate(text, out var cont))
                    WriteLine($"Error: {trans.Error}");

                if (Verbose)
                    WriteLine(trans.ToString());

                Assert.IsFalse(trans.Failed);
                _Exec.Continue(cont);
                while (_Exec.Next())
                    ;
            }
            catch (Exception e)
            {
                WriteLine($"Script {fileName}: Exception={e.Message}");
                return false;
            }

            return true;
        }

        protected bool RunScript(string scriptName)
            => RunScriptPathname(GetFullScriptPathname(scriptName));

        protected string GetScriptsPath()
            => MakeLocalPath(ScriptsFolder);

        private string MakeLocalPath(string relative)
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

        private void TestFreezeThawRho(string text)
            => Assert.IsTrue(Continue(FreezeThaw(_rho, text)));

        protected object Pop()
        {
            if (Verbose && _Exec.DataStack.Count == 0)
                _Exec.DebugTrace();
            Assert.Greater(DataStack.Count, 0, "Empty Datastack");
            return DataStack.Pop();
        }

        protected T Pop<T>()
        {
            var top = Pop();
            if (top is T result)                // Deal with unwrapped values.
                return result;

            var typed = top as IConstRef<T>;    // Deal with boxed values.
            if (typed == null)
                throw new TypeMismatchError(typeof(T), top.GetType());

            return typed.Value;
        }

        protected static void WriteLine(string fmt, params object[] args)
        {
            var text = fmt;
            if (args != null && args.Length > 0)
                text = string.Format(fmt, args);

            DebugTraceLine(text);
        }

        protected static void DebugTraceLine(string text)
        {
            TestContext.Out.WriteLine(text);
            System.Diagnostics.Trace.WriteLine(text);
            Console.WriteLine(text);
        }

        protected static PiLexer PiLex(string input)
        {
            var lex = new PiLexer(input);
            if (lex.Failed)
                WriteLine("LexerFailed: {0}", lex.Error);

            Assert.IsTrue(lex.Process(), lex.Error);
            return lex;
        }

        protected void AssertVarEquals<T>(string ident, T val)
        {
            Assert.IsTrue(_Scope.ContainsKey(ident), $"{ident} not found");
            var obj = _Scope[ident];
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

        private static void AssertSameTokens(IEnumerable<object> input, params EPiToken[] tokens)
        {
            var piTokens = input.Cast<PiToken>().Where(t => !IsWhiteSpace(t)).Select(t => t.Type).ToList();
            var expected = tokens.ToList();
            Assert.AreEqual(piTokens.Count, expected.Count);
            Assert.IsTrue(piTokens.SequenceEqual(expected));
        }

        protected static bool IsWhiteSpace(PiToken piToken)
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
            if (DataStack.Count != 0)
            {
                var sb = new StringBuilder();
                _Exec.WriteDataStack(sb, 10);
                WriteLine(sb.ToString());
            }
            Assert.AreEqual(0, DataStack.Count, $"Stack not empty {DataStack.Count} remain.");
            DataStack.Clear();
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
                _Exec.Continue(cont);
                return true;
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
                return false;
            }
        }

        private Continuation FreezeThaw(ITranslator trans, string text)
        {
            void Noisey(string info)
            {
                //if (Verbose)
                //    WriteLine(info);
            }
            Noisey("--- Input:");
            Noisey(text);
            Assert.IsTrue(trans.Translate(text, out var cont));

            Noisey("--- Serialised:");
            var str = cont.ToText();
            Noisey(str);
            Assert.IsNotEmpty(str);

            var thawed = PiTranslate(str);
            Assert.IsNotNull(thawed);
            var continuation = thawed.Code[0] as Continuation;
            Assert.IsNotNull(continuation);
            return continuation;
        }
        
        private string GetFullScriptPathname(string scriptName)
            => Path.Combine(GetScriptsPath(), scriptName);

        private string LoadScript(string fileName)
            => File.ReadAllText(GetFullScriptPathname(fileName));
    }
}

