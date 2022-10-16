namespace Pyro.Language {
    using System.Collections.Generic;

    /// <summary>
    /// Due to the limit that C# generic types can only have
    /// default constructors, we need this in-between factory
    /// type to provide ability to make an manipulate Nodes.
    /// </summary>
    public interface IAstFactory<in TTokenNode, TAstNode, in TEAstEnum> {
        void AddChild(TAstNode parent, TAstNode node);
        IList<TAstNode> GetChildren(TAstNode node);
        TAstNode New(TTokenNode t);
        TAstNode New(TEAstEnum t);
        TAstNode New(TEAstEnum e, TTokenNode t);
    }
}

