using Diver;
using Diver.Language;
using Diver.Language.Impl;
using NUnit.Framework;

namespace Pyro.Unity
{
    //public class UnityParser
    //    : ParserCommon<UnityLexer, AstNode, Token, EToken, EAst, AstFactory>
    //        , IParser
    //{
    //    protected UnityParser(UnityLexer lexer, IRegistry reg) : base(lexer, reg)
    //    {
    //    }
    //}
}

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var scenePath = @"C:\Users\chris\work\TemplateUnityProject\NewProject\Assets\Scenes\Main.unity";
            var contents = System.IO.File.ReadAllText(scenePath);
        }
    }
}