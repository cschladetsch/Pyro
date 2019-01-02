namespace Diver.Language
{
    public enum EStructure
    {
        None,
        Module,          // a self-contianed collection of reflected things
        Program,         // deprecated. to be replaced by module/namespace
        Statement,       // a simple statement with no context
        Expression,      // an expression using operators
        Namespace,       // a scoped collection of things
        Class,           // a collection of properties, events, and methods
    }
}