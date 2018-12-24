namespace Diver.Language
{
    public class RhoTranslator : ProcessCommon
    {
        protected RhoTranslator(IRegistry r) : base(r)
        {
        }

        public override string ToString()
        {
            return $"=== RhoTranslator:\nInput: {_lexer.Input}Lexer: {_lexer}\nParser: {_parser}";
        }

        private RhoLexer _lexer;
        private RhoParser _parser;
    }
}
