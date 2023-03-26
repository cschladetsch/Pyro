namespace Pyro.Language.Tau {
    using Impl;
    using Lexer;
    using Parser;

    /// <summary>
    /// Creates both agents and proxies from a single Parser AST tree
    /// </summary>
    public class TauTranslator
        : TranslatorBase<TauLexer, TauParser> {
        public TauTranslator(IRegistry registry) : base(registry) {
        }
    }
}