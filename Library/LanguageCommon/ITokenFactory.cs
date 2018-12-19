namespace Diver
{
    /// <summary>
    /// Required to make tokens based on different enumeration types.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TToken"></typeparam>
    public interface ITokenFactory<in TEnum, out TToken>
        where TToken : class, new()
    {
        TToken NewToken(LexerBase lexer, TEnum en, int lineNumber, Slice slice);
        TToken NewToken(LexerBase lexer, Slice slice);
        TToken NewTokenIdent(LexerBase lexer, int lineNumber, Slice slice);
        TToken NewTokenString(LexerBase lexer, int lineNumber, Slice slice);
        TToken NewEmptyToken(LexerBase lexer, int lineNumber, Slice slice);
    }
}