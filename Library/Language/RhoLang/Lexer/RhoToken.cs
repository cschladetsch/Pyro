using System.Security.Permissions;

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

        public override string ToString()
        {
            if (Slice.Length == 0 || string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(Text.Trim()))
                return Type.ToString();

            return $"{Type}: '{Text}'";
        }
    }
}
