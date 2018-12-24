namespace Diver.Language
{
    public class RhoToken : ITokenNode<ERhoToken>, ITokenBase<ERhoToken>
    {
        public ERhoToken Type { get; set; }
        public int LineNumber { get; }
        public Slice Slice { get; }
        public LexerBase Lexer { get; }
    }
}
