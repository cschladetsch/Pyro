using System;
using System.Collections.Generic;
using System.IO;

using Pyro.Exec;
using Pyro.Impl;
using Pyro.Language;
using Pyro.RhoLang;

namespace Pyro.ExecutionContext
{
    /// <summary>
    /// Functionality to execute scripts in any system-supported language
    /// given text or a filename.
    /// </summary>
    public class Context
        : Process
    {
        public IRegistry Registry => _registry;
        public ITranslator Translator => _translator;
        public Executor Executor => _exec;
        public IDictionary<string, object> Scope
        {
            get => _exec.Scope;
            set => _exec.Scope = value;
        }

        public ELanguage Language
        {
            get
            {
                if (_translator == _pi)
                    return ELanguage.Pi;
                return _translator == _rho ? ELanguage.Rho : ELanguage.None;
            }
            set
            {
                switch (value)
                {
                    case ELanguage.None:
                        _translator = null;
                        return;
                    case ELanguage.Pi:
                        _translator = _pi;
                        break;
                    case ELanguage.Rho:
                        _translator = _rho;
                        break;
                }
            }
        }

        public Context()
        {
            _registry = new Registry();
            _exec = _registry.Add(new Executor()).Value;
            RegisterTypes.Register(_registry);
            _pi = new PiTranslator(_registry);
            _rho = new RhoTranslator(_registry);
            Language = ELanguage.Pi;
        }

        public bool Exec(string text)
        {
            return _translator == null ? Fail("No translator") : Exec(_translator, text);
        }

        public bool Translate(string text, out Continuation result)
        {
            return Translate(_translator, out result, text);
        }

        private bool Translate(ITranslator translator, out Continuation result, string text)
        {
            result = null;
            return translator.Translate(text, out result) || Fail(translator.Error);
        }

        private bool Exec(ITranslator translator, string text)
        {
            try
            {
                if (!Translate(translator, out var cont, text))
                    return Fail(translator.Error);
                cont.Scope = _exec.Scope;
                _exec.Continue(cont);
            }
            catch (Exception e)
            {
                return Fail(e.Message);
            }
            return true;
        }

        public bool ExecRho(string text)
        {
            return Exec(ELanguage.Rho, text);
        }

        public bool ExecPi(string text)
        {
            return Exec(ELanguage.Pi, text);
        }

        public bool ExecFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return Fail("Empty filename");
            if (!File.Exists(fileName))
                return Fail($"File {fileName} doesn't exist");
            var ext = Path.GetExtension(fileName);
            switch (ext)
            {
                case ".rho":
                    return ExecPi(File.ReadAllText(fileName));
                case ".pi":
                    return ExecRho(File.ReadAllText(fileName));
                default:
                    return Fail($"Unrecognised extension {ext}");
            }
        }

        public bool Exec(ELanguage lang, string text)
        {
            switch (lang)
            {
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

        private readonly IRegistry _registry;
        private readonly Executor _exec;
        private readonly PiTranslator _pi;
        private readonly RhoTranslator _rho;
        private ITranslator _translator;
    }
}
