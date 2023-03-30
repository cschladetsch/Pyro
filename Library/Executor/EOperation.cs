namespace Pyro.Exec {
    /// <summary>
    ///     Operations that can be performed by an Executor,
    ///     which is the virtual machine used by all custom languages.
    ///     NOTE: Do NOT randomly re-order these values. They may be stored as integer values elsewhere.
    ///     NOTE: Only ever add to the end of this list.
    /// </summary>
    public enum EOperation {
        Nop
        ,

        // Instances
        New
        , Self
        , Delete
        , HasType
        , GetType
        , IsTypeOf
        , GetBaseTypes
        , Exists
        , GarbageCollect
        ,

        // Arithmetic
        Plus
        , Minus
        , Multiply
        , Floor
        , Ceiling
        , Divide
        , Modulo
        ,

        // Iteration
        ForEachIn
        , ForLoop
        ,

        // Variables
        Store
        , Has
        , Retrieve
        , Assign
        , GetPath
        ,

        // Flow control
        Suspend
        , Resume
        , Replace
        , Break
        , DataToContext
        , ContextToData
        ,

        // Output
        Assert
        , Write
        , WriteLine
        ,

        // Conditional
        If
        , IfElse
        ,

        // Stack
        StackToList
        , ListToStack
        , Depth
        , Dup
        , Clear
        , Swap
        , Rot
        , RotN
        , Roll
        , RollN
        , Pick
        , Over
        , Drop
        , DropN
        ,

        // Serialisation
        Freeze
        , Thaw
        , FreezeText
        , ThawText
        , FreezeYaml
        , ThawYaml
        ,

        // Logical
        Not
        , Equiv
        , NotEquiv
        , LogicalAnd
        , LogicalOr
        , LogicalXor
        , Less
        , Greater
        , GreaterOrEquiv
        , LessOrEquiv
        ,

        // Containers
        Expand
        , ToArray
        , ToMap
        , ToSet
        , ToPair
        , Size
        , GetBack
        , PushBack
        , PushFront
        , ToList
        , Remove
        , Insert
        , At
        ,

        // Accessors
        GetMember
        , SetMember
        , SetMemberValue
        ,

        // Debugging
        DebugPrint
        , DebugPrintDataStack
        , DebugPrintContextStack
        , DebugPrintContinuation
        , DebugSetLevel
        , DebugBreak
        , DebugSetVerbosity
        , DebugGetVerbosity
        ,

        // Global State
        SetFloatPrecision
        , DivEquals
        , MulEquals
        , MinusEquals
        , PlusEquals
    }
}