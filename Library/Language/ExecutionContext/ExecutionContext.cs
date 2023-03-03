namespace Pyro.ExecutionContext {
    using Exec;
    using Impl;
    using Language;
    using RhoLang;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <inheritdoc />
    /// <summary>
    /// Functionality to execute scripts in any system-supported language
    /// given text or a filename.
    /// </summary>
    public class ExecutionContext
        : Process {
        public IRegistry Registry { get; }
        public ITranslator Translator { get; private set; }
        public Executor Executor { get; }

        private readonly PiTranslator _pi;
        private readonly RhoTranslator _rho;

        public IDictionary<string, object> Scope
        {
            get => Executor.Scope;
            set => Executor.Scope = value;
        }

        public ELanguage Language
        {
            get
            {
                if (Translator == _pi)
                    return ELanguage.Pi;

                return Translator == _rho ? ELanguage.Rho : ELanguage.None;
            }
            set
            {
                switch (value) {
                    case ELanguage.None:
                        Translator = null;
                        return;

                    case ELanguage.Pi:
                        Translator = _pi;
                        break;

                    case ELanguage.Rho:
                        Translator = _rho;
                        break;

                    case ELanguage.Tau:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public bool ExecRho(string text)
            => Exec(ELanguage.Rho, text);

        public bool ExecPi(string text)
            => Exec(ELanguage.Pi, text);

        public bool Exec(string text)
            => Translator == null ? Fail("No translator") : Exec(Translator, text);

        public bool Translate(string text, out Continuation result)
            => Translate(Translator, out result, text);

        public ExecutionContext(bool runStartScripts = false) {
            Registry = new Registry();
            Executor = Registry.Add(new Executor()).Value;
            RegisterTypes.Register(Registry);
            _pi = new PiTranslator(Registry);
            _rho = new RhoTranslator(Registry);
            Language = ELanguage.Pi;

            if (runStartScripts)
                RunStartScripts();
        }

        private void RunStartScripts() {
            var pyroRoot = Path.Combine(HomePath(), ".pyro");
            void Exec(string file) => ExecFile(Path.Combine(pyroRoot, file));
            Exec("start.pi");
            Exec("start.rho");
        }

        protected string HomePath() {
            return (Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        }

        private bool Translate(ITranslator translator, out Continuation result, string text) {
            result = null;
            return translator.Translate(text, out result) || Fail(translator.Error);
        }

        private bool Exec(ITranslator translator, string text) {
            try {
                if (!Translate(translator, out var cont, text))
                    return Fail(translator.Error);

                cont.Scope = Executor.Scope;
                Executor.Continue(cont);
            } catch (Exception e) {
                return Fail(e.Message);
            }

            return true;
        }

        public bool ExecFile(string fileName) {
            if (!File.Exists(fileName))
                return Fail($"File '{fileName}' doesn't exist");

            var ext = Path.GetExtension(fileName);
            switch (ext) {
                case ".rho":
                    return ExecPi(File.ReadAllText(fileName));

                case ".pi":
                    return ExecRho(File.ReadAllText(fileName));

                default:
                    return Fail($"Unrecognised extension {ext}");
            }
        }

        public bool Exec(ELanguage lang, string text) {
            switch (lang) {
                case ELanguage.None:
                    return Fail("No language selected");

                case ELanguage.Pi:
                    return Exec(_pi, text);

                case ELanguage.Rho:
                    return Exec(_rho, text);

                default:
                    throw new ArgumentOutOfRangeException(nameof(lang), lang, null);
            }
        }
    }
}

