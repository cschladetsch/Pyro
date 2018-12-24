namespace Diver.Language
{
    /// <summary>
    /// Parser for the in-fix Rho language that uses tabs for block definitions like Python.
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

            if (_lexer.Failed)
                return Fail(_lexer.Error);

            //RemoveWhitespace()n

            //return Run(structure);

            return false;
        }
    }
}
