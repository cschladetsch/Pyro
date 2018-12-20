namespace Diver.LanguageCommon
{
    public enum EStructure
    {
        None,
        Single,          // an expression or such
        Sequence,        // a contaniner or such
        Module,          // a self-contianed collection of reflected things
        Statement,       // a simple statement with no context
        Expression,      // an expression using operators
        Function,        // a thing that gives output given some or no input
        Program,         // deprecated. to be replaced by module/namespace
        Namespace,       // a scoped collection of things
        Class,           // a collection of properties, events, and methods
    }
}