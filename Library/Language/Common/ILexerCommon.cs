namespace Pyro.Language {
    /// <summary>
    ///     Interface common to all lexers that use the given TokenType.
    /// </summary>
    /// <typeparam name="TToken">The token type used by the lexer.</typeparam>
    public interface ILexerCommon<in TToken>
        : ILexer {
        string CreateErrorMessage(TToken tok, string fmt, params object[] args);
    }
}