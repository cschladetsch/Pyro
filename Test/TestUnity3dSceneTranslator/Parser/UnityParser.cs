using Diver;
using Diver.Language;
using Diver.Language.Impl;

namespace Pyro.Unity
{
    public class UnityParser
        : ParserCommon<UnityLexer, AstNode, Token, EToken, EAst, AstFactory>
            , IParser
    {
        protected UnityParser(UnityLexer lexer, IRegistry reg) : base(lexer, reg)
        {
        }
    }
}