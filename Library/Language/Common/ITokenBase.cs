namespace Pyro.Language {
    /// <summary>
    ///     Common for all tokens.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to specify a token type.</typeparam>
    public interface ITokenBase<TEnum> {
        /// <summary>
        ///     The type of the token.
        /// </summary>
        TEnum Type { get; set; }

        /// <summary>
        ///     The lexer that made this token.
        ///     TODO: Use ILexer
        /// </summary>
        LexerBase Lexer { get; }

        /// <summary>
        ///     The line number of the token in the input text range.
        /// </summary>
        int LineNumber { get; }

        /// <summary>
        ///     Where in the input text range this token exists.
        /// </summary>
        Slice Slice { get; }
    }
}