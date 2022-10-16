namespace Pyro.Language {
    /// <summary>
    /// Common for all nodes in an Ast tree that simply represents a token.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public interface ITokenNode<out TEnum> {
        /// <summary>
        /// The type of the token this Ast node represents.
        /// </summary>
        TEnum Type { get; }
    }
}

