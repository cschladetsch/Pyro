using Pyro.Language.Impl;
using Pyro.Language.Tau.Lexer;
using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    /// <summary>
    ///     Creates both agents and proxies from a single Parser AST tree
    /// </summary>
    public class TauTranslator
        : TranslatorBase<TauLexer, TauParser> {
        public TauTranslator(IRegistry registry) : base(registry) {
        }
    }
}