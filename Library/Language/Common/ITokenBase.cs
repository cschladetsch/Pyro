namespace Pyro.Language
{
    public interface ITokenBase<TEnum>
    {
        TEnum Type { get; set; }
        int LineNumber { get; }
        Slice Slice { get; }
        LexerBase Lexer { get; }
    }
}