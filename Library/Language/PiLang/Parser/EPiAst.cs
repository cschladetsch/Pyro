namespace Pyro.Language.Parser {
    /// <summary>
    ///     Abstract Syntax Tree (Ast) node types.
    /// </summary>
    public enum EPiAst {
        None
        , Program
        , Operation
        , List
        , Map
        , Set
        , Array
        , Continuation
        , TokenType
        , Ident
        , Pathname
        , Object
    }
}