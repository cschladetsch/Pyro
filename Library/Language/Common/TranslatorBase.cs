using Diver.Exec;

namespace Diver.Language
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

        protected TranslatorBase(IRegistry reg, Executor exec) : this(reg)
        {
            _executor = exec;
        }

        public override bool Translate(string text, EStructure st = EStructure.Program)
        {
            return false;
        }

        protected TLexer _lexer;
        protected TParser _parser;
        protected Executor _executor;
    }

    public interface IParser
    {
    }
}