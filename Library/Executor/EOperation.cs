namespace Diver.Exec
{
    public enum EOperation
    {
        Plus,
        Minus,
        Multiply,
        Divide,

        Store,
        Retrieve,

        Suspend,
        Resume,
        Replace,
        
        Print,
        Assert,

        If,
        IfThen,
        IfThenElse,

        ToArray,
        ToMap,
        ToSet,
        ToPair,

        Expand,
        Dup,
        Clear,
        Swap
    }
}