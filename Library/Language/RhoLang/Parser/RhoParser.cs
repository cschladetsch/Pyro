namespace Diver.Language
{
    /// <summary>
    /// Parser for the Pi language. It's quite simple.
    /// </summary>
    public class RhoParser
        : ParserCommon<RhoLexer, RhoAstNode, RhoToken, ERhoToken, ERhoAst, RhoAstFactory>
    {
        public RhoParser(LexerBase lexer)
            : base(lexer, null)
        {
        }

        public override bool Process(RhoLexer lex, EStructure structure = EStructure.None)
        {
            _current = 0;
            _indent = 0;
            _lexer = lex;

            //if (_lexer.Failed)
            //    return Fail(_lexer.Error);

            //RemoveWhitespace();

            //return Run(structure);

            return false;
        }
    }
}
