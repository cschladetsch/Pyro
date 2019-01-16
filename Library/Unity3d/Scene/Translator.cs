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

    public static class Translate
    {
        public static GameObject From(string pathName)
        {
            return null;
        }

        public static string To(GameObject root)
        {
            return null;
        }
    }
}
