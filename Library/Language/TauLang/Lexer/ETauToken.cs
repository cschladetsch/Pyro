namespace Pyro.Language.Tau.Lexer
{
    public enum ETauToken
    {
        None,
        Namespace,
        OpenBrace,
        CloseBrace,
        OpenParan,
        CloseParan,
        Class,
        Ident,
        Int,
        String,
        Float,
        Getter,
        Setter,
        Semi,
        Comma,

        List,
        Set,
        Dict,

        End,
    }
}