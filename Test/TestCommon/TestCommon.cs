using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Pyro.Exec;
using Pyro.Impl;
using Pyro.Language;
using Pyro.Language.Lexer;
using Pyro.RhoLang;
using RegisterTypes = Pyro.Exec.RegisterTypes;

namespace Pyro.Test {
    /// <inheritdoc />
    /// <summary>
    ///     Common to most unit tests in the system.
    /// </summary>
    [TestFixture]
    public class TestCommon
        : TestCommonBase {
        [SetUp]
        public void Setup() {
            _Registry = new Registry();
            RegisterTypes.Register(_Registry);

            _pi = new PiTranslator(_Registry);
            _rho = new RhoTranslator(_Registry);
            _ExecutorRef = _Registry.Add(new Executor());
        }

        protected bool Verbose = true;

        private Continuation Continuation { get; set; }

        protected IDictionary<string, object> _Scope => Continuation?.Scope;

        protected Stack<object> DataStack => _Exec.DataStack;

        protected IRegistry _Registry;

        private IRef<Executor> _ExecutorRef;

        protected Executor _Exec => _ExecutorRef.Value;

        private ITranslator _pi;
        private ITranslator _rho;

        protected void PiRun(string text, bool trace = false) {
            Continuation = PiTranslate(text, trace);
            Run(trace);
        }

        protected void RhoRun(string text, bool trace = false, EStructure st = EStructure.Program) {
            Continuation = RhoTranslate(text, trace, st);
            Run(trace);
        }

        private void Run(bool trace = false) {
            _Exec.Clear();
            if (trace) {
                WriteLine(Continuation.ToString());
            }

            _Exec.Continue(Continuation);
        }

        private Continuation PiTranslate(string piScript, bool trace = false) {
            var trans = new PiTranslator(_Registry);
            if (!trans.Translate(piScript, out var cont)) {
                WriteLine($"Error: {trans.Error}");
            }

            if (trace) {
                WriteLine(cont.ToString());
            }

            Assert.IsFalse(trans.Failed, trans.Error);
            return Continuation = cont;
        }

        protected Continuation RhoTranslate(string rhoScript, bool trace = false, EStructure st = EStructure.Program) {
            var trans = new RhoTranslator(_Registry);
            if (!trans.Translate(rhoScript, out var cont, st)) {
                WriteLine($"Error: {trans.Error}");
            }

            if (trace) {
                WriteLine(trans.ToString());
            }

            Assert.IsFalse(trans.Failed, trans.Error);
            return Continuation = cont;
        }

        private ITranslator MakeTranslator(string scriptName) {
            Type klass = null;
            var extension = Path.GetExtension(scriptName);
            switch (extension) {
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

        protected bool RunScriptPathname(string filePath) {
            var fileName = Path.GetFileName(filePath);
            try {
                WriteLine($"********************** Running {fileName}");
                _Exec.SourceFilename = fileName;
                var text = File.ReadAllText(filePath);
                var trans = MakeTranslator(filePath);
                if (!trans.Translate(text, out var cont)) {
                    WriteLine($"Error: {trans.Error}");
                }

                if (Verbose) {
                    WriteLine(trans.ToString());
                }

                Assert.IsFalse(trans.Failed);
                _Exec.Continue(cont);
            } catch (Exception e) {
                WriteLine($"Script {fileName}: Exception={e.Message}");
                return false;
            }

            return true;
        }

        protected bool RunScript(string scriptName) {
            return RunScriptPathname(GetFullScriptPathname(scriptName));
        }


        protected void AssertEmpty() {
            Assert.AreEqual(0, DataStack.Count);
        }

        protected void AssertPop<T>(T val) {
            Assert.AreEqual(val, Pop<T>());
        }

        protected T ConstRef<T>(object o) {
            return Executor.ConstRef<T>(o);
        }

        protected void TestFreezeThawPi(string text) {
            Assert.IsTrue(Continue(FreezeThaw(_pi, text)));
        }

        private void TestFreezeThawRho(string text) {
            Assert.IsTrue(Continue(FreezeThaw(_rho, text)));
        }

        protected object Pop() {
            if (Verbose && _Exec.DataStack.Count == 0) {
                _Exec.DebugTrace();
            }

            Assert.Greater(DataStack.Count, 0, "Empty Datastack");
            return DataStack.Pop();
        }

        protected T Pop<T>() {
            var top = Pop();
            if (top is T result) // Deal with unwrapped values.
            {
                return result;
            }

            var typed = top as IConstRef<T>; // Deal with boxed values.
            if (typed == null) {
                throw new TypeMismatchError(typeof(T), top.GetType());
            }

            return typed.Value;
        }

        protected static PiLexer PiLex(string input) {
            var lex = new PiLexer(input);
            if (lex.Failed) {
                WriteLine("LexerFailed: {0}", lex.Error);
            }

            Assert.IsTrue(lex.Process(), lex.Error);
            return lex;
        }

        protected void AssertVarEquals<T>(string ident, T val) {
            Assert.IsTrue(_Scope.ContainsKey(ident), $"{ident} not found");
            var obj = _Scope[ident];
            switch (obj) {
                case T v:
                    Assert.AreEqual(v, val);
                    return;
                case IRefBase rb:
                    Assert.AreEqual(rb.BaseValue, val);
                    return;
            }
        }

        protected void AssertSameTokens(string input, params EPiToken[] tokens) {
            var lex = PiLex(input);
            AssertSameTokens(lex.Tokens, tokens);
        }

        private static void AssertSameTokens(IEnumerable<object> input, params EPiToken[] tokens) {
            var piTokens = input.Cast<PiToken>().Where(t => !IsWhiteSpace(t)).Select(t => t.Type).ToList();
            var expected = tokens.ToList();
            Assert.AreEqual(piTokens.Count, expected.Count);
            Assert.IsTrue(piTokens.SequenceEqual(expected));
        }

        protected static bool IsWhiteSpace(PiToken piToken) {
            switch (piToken.Type) {
                case EPiToken.Whitespace:
                case EPiToken.Tab:
                case EPiToken.NewLine:
                    return true;
            }

            return false;
        }

        protected void TestScript(string scriptName) {
            WriteLine($"Testing script {scriptName}.");
            Assert.IsTrue(RunScript(scriptName), $"Script={scriptName}");
            if (DataStack.Count != 0) {
                var sb = new StringBuilder();
                _Exec.WriteDataStack(sb, 10);
                WriteLine(sb.ToString());
            }

            Assert.AreEqual(0, DataStack.Count, $"Stack not empty {DataStack.Count} remain.");
            DataStack.Clear();
        }

        protected void FreezeThaw(string fileName) {
            var script = LoadScript(fileName);
            switch (Path.GetExtension(fileName)) {
                case ".pi":
                    TestFreezeThawPi(script);
                    return;

                case ".rho":
                    TestFreezeThawRho(script);
                    return;
            }

            Assert.Fail($"Unsupported extension {fileName}");
        }

        private bool Continue(Continuation cont) {
            Assert.IsNotNull(cont);
            try {
                _Exec.Continue(cont);
                return true;
            } catch (Exception e) {
                WriteLine(e.Message);
                return false;
            }
        }

        private Continuation FreezeThaw(ITranslator trans, string text) {
            void Noisey(string info) {
                if (Verbose) {
                    WriteLine(info);
                }
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
    }
}