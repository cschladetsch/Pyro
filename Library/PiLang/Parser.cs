using Diver.LanguageCommon;

namespace Diver.PiLang
{
    class Parser : ParserCommon<Lexer, AstNode, Token, EToken, EAstNode, AstFactory>
    {
        protected Parser(LexerBase lexer, IRegistry r) : base(lexer, r)
        {
        }
    }
}
