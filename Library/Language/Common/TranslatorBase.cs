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

        protected override Continuation Translate(
            string text, EStructure st = EStructure.Statement)
        {
            return null;
        }

        protected TLexer _lexer;
        protected TParser _parser;
        protected Executor _executor;
    }

    public interface IParser
    {
    }
}