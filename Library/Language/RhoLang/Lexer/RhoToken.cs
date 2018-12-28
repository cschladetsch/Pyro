namespace Diver.Language
{
    public class RhoToken
        : TokenBase<ERhoToken>
        , ITokenNode<ERhoToken>
    {
        public RhoToken()
        {
            _type = ERhoToken.None;
        }

        public RhoToken(ERhoToken type, Slice slice)
            : base(type, slice)
        {
        }
    }
}
