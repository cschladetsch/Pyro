namespace Pyro.Language.Lexer
{
    using Impl;

    /// <inheritdoc cref="ITokenBase{TEnum}" />
    /// <summary>
    /// A Pi-lang token.
    /// </summary>
    public class PiToken
        : TokenBase<EPiToken>
        , ITokenNode<EPiToken>
    {
        public PiToken() => _type = EPiToken.None;

        public PiToken(EPiToken type, Slice slice)
            : base(type, slice)
        {
        }
    }
}

