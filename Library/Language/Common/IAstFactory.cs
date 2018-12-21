using System.Collections.Generic;

namespace Diver.Language
{
    /// <summary>
    /// Due to the limit that C# generic types can only have
    /// default constructors, we need this in-between factory
    /// type to provide ability to make an manipulate Nodes.
    /// </summary>
    /// <typeparam name="TTokenNode"></typeparam>
    /// <typeparam name="TAstNode"></typeparam>
    /// <typeparam name="EAstEnum"></typeparam>
    public interface IAstFactory<in TTokenNode, TAstNode, in EAstEnum>
    {
        void AddChild(TAstNode parent, TAstNode node);
        void AddChild(TAstNode parent, object node);
        TAstNode New(TTokenNode t);
        TAstNode New(EAstEnum e, TTokenNode t);
        TAstNode New(EAstEnum t);
        IList<TAstNode> GetChildren(TAstNode node);
    }
}