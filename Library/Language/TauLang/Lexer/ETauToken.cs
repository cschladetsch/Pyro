namespace Pyro.TauLang.Lexer { 
    public enum ETauToken {
        None,
        Nop,

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

        NewLine,
        Comment,
        Space,
        Tab,

        End,
    }
}