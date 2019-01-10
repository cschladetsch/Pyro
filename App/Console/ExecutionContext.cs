using System;
using System.Collections.Generic;
using System.IO;
using Diver;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;

namespace Console
{
    /// <summary>
    /// Functionality to execute scripts in any system-supported language
    /// given text or a filename.
    /// </summary>
    public class ExecutionContext : Process
    {
        public IRegistry Registry => _registry;
        public ITranslator Ttranslator => _translator;
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

                Language = value;
            }
        }

        public ExecutionContext()
        {
            _registry = new Registry();
            _exec = _registry.Add(new Diver.Exec.Executor()).Value;
            _pi = new PiTranslator(_registry);
            _rho = new RhoTranslator(_registry);
        }

        public bool Exec(string text)
        {
            return _translator == null ? Fail("No translator") : Exec(_translator, text);
        }

        private bool Exec(ITranslator translator, string text)
        {
            try
            {
                if (!translator.Translate(text))
                    return Fail(translator.Error);
                var cont = translator.Result;
                cont.Scope = _exec.Scope;
                _exec.Continue(translator.Result);
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

        private bool Exec(ELanguage pi, string text)
        {
            throw new NotImplementedException();
        }

        private readonly IRegistry _registry;
        private readonly Executor _exec;
        private readonly PiTranslator _pi;
        private readonly RhoTranslator _rho;
        private ITranslator _translator;
    }
}