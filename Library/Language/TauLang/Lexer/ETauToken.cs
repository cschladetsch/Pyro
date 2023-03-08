namespace Pyro.Language.Tau.Lexer { 
    public enum ETauToken {
        None,
        Nop,

        Namespace,
        OpenBrace,
        CloseBrace,
        Interface,
        
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
        WhiteSpace,
        Tab,
    }
}