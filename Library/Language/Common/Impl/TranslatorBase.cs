namespace Pyro.Language.Impl {
    /// <inheritdoc />
    /// <summary>
    ///     Common for all Language Parsers.
    /// </summary>
    /// <typeparam name="TLexer">The lexicographical analyser to use.</typeparam>
    /// <typeparam name="TParser">The Abstract Syntax Tree (Ast) generator to use.</typeparam>
    public class TranslatorBase<TLexer, TParser>
        : TranslatorCommon
        where TLexer
        : ILexer
        where TParser
        : IParser {
        protected TLexer _Lexer;
        protected TParser _Parser;

        protected TranslatorBase(IRegistry reg)
            : base(reg) {
        }

        public TParser Parser => _Parser;
    }
}