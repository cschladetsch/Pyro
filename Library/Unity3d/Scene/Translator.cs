﻿using Diver;
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
        public static SceneInstance From(string pathName)
        {
            return null;
        }

        public static string To(SceneInstance root)
        {
            return null;
        }
    }
}
