namespace Pyro.Unity3d.Scene.Lexer
{
    public enum EToken
    {
        None,

        Space,
        Indent,
        NewLine,
        Return,
        Tab,

        Bool,
        Int,
        Float,
        String,
        Ident,

        Percent,
        Dash,
        DashDashDash,
        Colon,
        Tag,
        Bang,
        Ampersand,
        Comma,

        OpenBrace,
        CloseBrace,
        OpenSquareBracket,
        CloseSquareBracket,
        OpenParan,
        CloseParan,
        Document,
        TAG,
        YAML
    }
}
