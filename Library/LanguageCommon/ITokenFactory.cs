namespace Diver
{
    /// <summary>
    /// Required to make tokens based on different enumeration types.
    ///
    /// This is necessary, as each C# enum derives from System.Enum,
    /// so we cannot show enum values across different language token sets.
    /// 
    /// </summary>
    /// <typeparam name="TEnum">The token types for the language</typeparam>
    /// <typeparam name="TToken">A token in the lexer</typeparam>
    public interface ITokenFactory<in TEnum, out TToken>
        where TToken : class, new()
    {
        TToken NewToken(TEnum en, int lineNumber, Slice slice);
        TToken NewTokenIdent(int lineNumber, Slice slice);
        TToken NewTokenString(int lineNumber, Slice slice);
        TToken NewEmptyToken(int lineNumber, Slice slice);
        void SetLexer(LexerBase lexer);
    }
}