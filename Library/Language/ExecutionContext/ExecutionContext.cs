﻿using System;
using System.Collections.Generic;
using System.IO;
using Pyro.Exec;
using Pyro.Impl;
using Pyro.Language;
using Pyro.RhoLang;

namespace Pyro.ExecutionContext {
    /// <inheritdoc />
    /// <summary>
    ///     Functionality to execute scripts in any system-supported language
    ///     given text or a filename.
    /// </summary>
    public class ExecutionContext
        : Process {
        private readonly PiTranslator _pi;
        private readonly RhoTranslator _rho;

        public ExecutionContext(bool runStartScripts = false) {
            Registry = new Registry();
            Executor = Registry.Add(new Executor()).Value;
            RegisterTypes.Register(Registry);
            _pi = new PiTranslator(Registry);
            _rho = new RhoTranslator(Registry);
            Language = ELanguage.Pi;

            if (runStartScripts) {
                RunStartScripts();
            }
        }

        public IRegistry Registry { get; }
        public ITranslator Translator { get; private set; }

        public Executor Executor { get; }
        //private readonly TauTranslator _tau;

        public IDictionary<string, object> Scope {
            get => Executor.Scope;
            set => Executor.Scope = value;
        }

        public ELanguage Language {
            get {
                if (Translator == _pi) {
                    return ELanguage.Pi;
                }

                return Translator == _rho ? ELanguage.Rho : ELanguage.None;
            }
            set {
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

        public bool ExecRho(string text) {
            return Exec(ELanguage.Rho, text);
        }

        private bool ExecPi(string text) {
            return Exec(ELanguage.Pi, text);
        }

        public bool Exec(string text) {
            return Translator == null ? Fail("No translator") : Exec(Translator, text);
        }

        public bool Translate(string text, out Continuation result) {
            return Translate(Translator, out result, text);
        }

        private void RunStartScripts() {
            var pyroRoot = Path.Combine(HomePath(), ".pyro");

            void Exec(string file) {
                ExecFile(Path.Combine(pyroRoot, file));
            }

            Exec("start.pi");
            Exec("start.rho");
        }

        private static string HomePath() {
            return Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.MacOSX
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        }

        private bool Translate(ITranslator translator, out Continuation result, string text) {
            return translator.Translate(text, out result) || Fail(translator.Error);
        }

        private bool Exec(ITranslator translator, string text) {
            try {
                if (!Translate(translator, out var cont, text)) {
                    return Fail(translator.Error);
                }

                cont.Scope = Executor.Scope;
                Executor.Continue(cont);
            } catch (Exception e) {
                Fail(e.Message);
                throw e;
            }

            return true;
        }

        private bool ExecFile(string fileName) {
            if (!File.Exists(fileName)) {
                return Fail($"File '{fileName}' doesn't exist");
            }

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

                case ELanguage.Tau:
                default:
                    throw new ArgumentOutOfRangeException(nameof(lang), lang, null);
            }
        }
    }
}