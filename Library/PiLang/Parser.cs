using Diver.LanguageCommon;

namespace Diver.PiLang
{
    class Parser : ParserCommon<Lexer, AstNode, Token, EToken, EAst, AstFactory>
    {
        protected Parser(LexerBase lexer, IRegistry r) : base(lexer, r)
        {
        }

        protected override bool Process(Lexer lex, EStructure st)
        {
            return false;
        }

        private bool Run(EStructure st)
        {
            return false;
        }

        private bool NextSingle(AstNode context)
        {
            return false;
        }

        private bool ParseArray(AstNode context)
        {
            return false;
        }

        private bool ParseContinuation(AstNode context)
        {
            return false;
        }

        private bool ParseCompound(AstNode root, EAst type, EToken end)
        {
            return false;
        }
    }
}
