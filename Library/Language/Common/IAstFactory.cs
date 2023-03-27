using System.Collections.Generic;

namespace Pyro.Language {

    /// <summary>
    /// Due to the limit that C# generic types can only have
    /// default constructors, we need this in-between factory
    /// type to provide ability to make an manipulate Nodes.
    /// </summary>
    public interface IAstFactory<in TTokenNode, TAstNode, in TEAstEnum> {

        void AddChild(TAstNode parent, TAstNode node);

        IList<TAstNode> GetChildren(TAstNode node);

        TAstNode New(TTokenNode tokenNode);

        TAstNode New(TEAstEnum astEnum);

        TAstNode New(TEAstEnum astEnum, TTokenNode tokenNode);
    }
}

