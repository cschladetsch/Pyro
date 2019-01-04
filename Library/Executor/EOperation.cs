namespace Diver.Exec
{
    /// <summary>
    /// Operations that can be performed by an Executor.
    ///
    /// This is the virtual machine that is used by all
    /// custom languages.
    /// </summary>
    public enum EOperation
    {
        Nop,

        // instances
        New,
        Delete,
        HasType,
        Exists,
        GarbageCollect,

        // arithmetic
        Plus,
        Minus,
        Multiply,
        Divide,
        Modulo,

        // variables
        Store,
        Has,
        Retrieve,
        Assign,
        GetPath,

        // flow control
        Suspend,
        Resume,
        Replace,
        
        // output
        Assert,
        Write,
        WriteLine,

        // conditional
        If,
        IfElse,

        // stack
        StackToList,
        ListToStack,
        Depth,
        Dup,
        Clear,
        Swap,
        Break,
        Rot,
        Roll,
        RotN,
        RollN,
        Pick,
        Over,

        // serialisation
        Freeze,
        Thaw,
        FreezeText,
        ThawText,
        FreezeYaml,
        ThawYaml,

        // logical
        Not,
        Equiv,
        LogicalAnd,
        LogicalOr,
        LogicalXor,
        Less,
        Greater,
        GreaterOrEquiv,
        LessOrEquiv,
        NotEquiv,

        // containers
        Expand,
        ToArray,
        ToMap,
        ToSet,
        ToPair,
        Size,
        GetBack,
        PushBack,
        PushFront,
        ToList,
        Remove,
        Insert,
        At,

        // debugging
        DebugPrintDataStack,
        DebugPrintContextStack,
        DebugPrint,
        DebugPrintContinuation,
        DebugSetLevel,

        SetFloatPrecision,

        Self,

        // accessors
        GetMember,
        SetMember,
        SetMemberValue,

    }
}