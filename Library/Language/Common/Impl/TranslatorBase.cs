using Pryo;
using Pyro.Exec;

namespace Pyro.Language.Impl
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

        public override bool Translate(string text, out Continuation cont, EStructure st = EStructure.Program)
        {
            return base.Translate(text, out cont, st);
        }

        protected TLexer _lexer;
        protected TParser _parser;
    }
}