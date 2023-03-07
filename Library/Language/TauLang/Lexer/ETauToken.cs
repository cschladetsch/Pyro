namespace Pyro.Language.Tau.Lexer {
    public enum ETauToken {
        None,

        Namespace,
        OpenBrace,
        CloseBrace,
        Class,

        Ident,
        OpenParan,
        CloseParan,

        Semi,
        Comma,

        Void,
        Int,
        String,
        Float,

        Getter,
        Setter,

        List,
        Set,
        Dict,

        Event,
        Func,
        LessThan,
        GreaterThan,

        End,
    }
}