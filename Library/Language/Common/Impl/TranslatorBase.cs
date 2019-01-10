using Diver.Exec;

namespace Diver.Language.Impl
{
    public class TranslatorBase<TLexer, TParser>
        : TranslatorCommon
        where TLexer : ILexer
        where TParser : IParser
    {
        public TLexer Lexer => _lexer;
        public TParser Parser => _parser;

        protected TranslatorBase(IRegistry reg) : base(reg)
        {
        }

        public override bool Translate(string text, EStructure st = EStructure.Program)
        {
            return base.Translate(text, st);
        }

        protected TLexer _lexer;
        protected TParser _parser;
    }
}