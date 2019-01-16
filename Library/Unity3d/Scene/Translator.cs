using Diver;
using Diver.Language.Impl;

namespace Pyro.Unity3d.Scene
{
    public class Translator
        : TranslatorBase<UnityLexer, UnityParser>
    {
        protected Translator(IRegistry reg) : base(reg)
        {
        }
    }
}
