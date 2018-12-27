namespace Diver.Exec
{
    /// <summary>
    /// Operations that can be performed by and Executor
    /// </summary>
    public enum EOperation
    {
        Plus,
        Minus,
        Multiply,
        Divide,

        Store,
        Has,
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
        Swap,
        Break,
        Not,
        Equiv,
        LogicalAnd,
        LogicalOr,
        LogicalXor,
        Size,
        GetBack,
        PushBack,
        PushFront,
        ToList,
        Remove,
        Insert,
        At,
        DebugPrintDataStack,
        DebugPrintContextStack,
        DebugPrint,
        DebugPrintContinuation,
        Depth
    }
}