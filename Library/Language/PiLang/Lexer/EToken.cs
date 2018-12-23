﻿namespace Diver.Language.PiLang
{
    public enum EToken
    {
        None,
        Integer,
        Float,
        Int,
        String,
        True,
        False,
        Ident,
        Exists,
        Version,
        OpenBrace,
        CloseBrace,
        OpenSquareBracket,
        CloseSquareBracket,
        Store,
        Suspend,
        Replace,
        Resume,
        Plus,
        Minus,
        Multiply,
        Divide,
        Equiv,
        Assign,
        NotEquiv,
        Less,
        Greater,
        LessEquiv,
        GreaterEquiv,
        Dup,
        Lookup,
        Comma,
        Drop,
        Over,
        Rot,
        Depth,
        Pick,
        Expand,
        ToArray,
        Name,
        Fullname,
        Freeze,
        Thaw,
        StringFreeze,
        StringThaw,
        GetType,
        This,
        GetContents,
        SetTraceLevel,
        GetTraceLevel,
        GetDebugLevel,
        SetDebugLevel,

        If,
        IfElse,
        For,
        Self,
        While,
        Assert,
        ToRho,
        ToRhoSequence,
        Not,
        And,
        Or,
        Xor,

        Swap,
        RotN,
        GarbageCollect,
        Clear,
        ChangeFolder,
        PrintFolder,
        ToList,
        ToMap,
        ToSet,

        Contents,
        Size,
        New,
        DropN,

        BitOr,
        Decrement,
        Increment,
        MinusAssign,
        Tab,
        NewLine,

        OpenParan,
        CloseParan,
        Colon,
        Whitespace,
        PlusAssign,
        Comment,
        Retrieve
    }
}