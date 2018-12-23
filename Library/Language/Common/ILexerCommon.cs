namespace Diver.Language
{
    public interface ILexerCommon<TToken>
    {
        string CreateErrorMessage(TToken tok, string fmt, params object[] args);
    }
}