namespace Pyro.Language
{
    public interface ILexerCommon<in TToken>: ILexer
    {
        string CreateErrorMessage(TToken tok, string fmt, params object[] args);
    }

    public interface ILexer : IProcess
    {
    }

    public interface ILexer<in TToken> : ILexer, ILexerCommon<TToken>
    {
    }
}