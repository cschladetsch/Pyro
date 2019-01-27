using Pryo;
using Pyro.Language;
using Pyro.Language.Impl;
using Pyro.Unity3d.Scene.Lexer;

namespace Pyro.Unity3d.Scene.Parser
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