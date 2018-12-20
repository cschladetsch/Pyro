namespace Diver.LanguageCommon
{
    public interface IAstFactory<in TTokenNode, TAstNode, in EAstEnum>
    {
        void AddChild(TAstNode parent, TAstNode node);
        void AddChild(TAstNode parent, object node);
        TAstNode New(TTokenNode t);
        //TAstNode New(TAstNode e, TTokenNode t);
        TAstNode New(EAstEnum e, TTokenNode t);
        TAstNode New(EAstEnum t);
    }
}