namespace Pyro.Language.Lexer {
    using Exec;

    public partial class PiLexer {
        protected override void AddKeyWords() {
            AddKeyword("break", EPiToken.Break);
            AddKeyword("if", EPiToken.If);
            AddKeyword("ife", EPiToken.IfElse);
            AddKeyword("for", EPiToken.For);
            AddKeyword("true", EPiToken.True);
            AddKeyword("false", EPiToken.False);
            AddKeyword("self", EPiToken.Self);
            AddKeyword("while", EPiToken.While);
            AddKeyword("assert", EPiToken.Assert);
            AddKeyword("div", EPiToken.Divide);
            AddKeyword("rho", EPiToken.ToRho);
            AddKeyword("rho{", EPiToken.ToRhoSequence);
            AddKeyword("not", EPiToken.Not);
            AddKeyword("and", EPiToken.And);
            AddKeyword("or", EPiToken.Or);
            AddKeyword("xor", EPiToken.Xor);
            AddKeyword("exists", EPiToken.Exists);
            AddKeyword("drop", EPiToken.Drop);
            AddKeyword("dup", EPiToken.Dup);
            AddKeyword("over", EPiToken.Over);
            AddKeyword("swap", EPiToken.Swap);
            AddKeyword("rot", EPiToken.Rot);
            AddKeyword("pick", EPiToken.Pick);
            AddKeyword("rotn", EPiToken.RotN);
            AddKeyword("gc", EPiToken.GarbageCollect);
            AddKeyword("clear", EPiToken.Clear);
            AddKeyword("cd", EPiToken.ChangeFolder);
            AddKeyword("pwd", EPiToken.PrintFolder);
            AddKeyword("type", EPiToken.GetType);
            AddKeyword("depth", EPiToken.Depth);
            AddKeyword("new", EPiToken.New);
            AddKeyword("dropn", EPiToken.DropN);
            AddKeyword("noteq", EPiToken.NotEquiv);
            AddKeyword("lls", EPiToken.Contents);
            AddKeyword("ls", EPiToken.GetContents);
            AddKeyword("freeze", EPiToken.Freeze);
            AddKeyword("thaw", EPiToken.Thaw);
            AddKeyword("size", EPiToken.Size);
            AddKeyword("tomap", EPiToken.ToMap);
            AddKeyword("toset", EPiToken.ToSet);
            AddKeyword("tolist", EPiToken.ToList);
            AddKeyword("toarray", EPiToken.ToArray);
            AddKeyword("expand", EPiToken.Expand);
            AddKeyword("remove", EPiToken.Remove);
            AddKeyword("push_front", EPiToken.PushFront);
            AddKeyword("push_back", EPiToken.PushBack);
            AddKeyword("back", EPiToken.GetBack);
            AddKeyword("at", EPiToken.At);
            AddKeyword("insert", EPiToken.Insert);
            AddKeyword("has", EPiToken.Has);
            AddKeyword("write", EPiToken.Write);
            AddKeyword("writeln", EPiToken.WriteLine);
            AddKeyword("debug_datastack", EPiToken.DebugPrintDataStack);
            AddKeyword("set_float_precision", EPiToken.SetFloatPrecision);
        }

        private void AddKeyword(string str, EPiToken tok) {
            _KeyWords.Add(str, tok);
            _KeyWordsInvert.Add(tok, str);
        }

        private static void CreateOpToToken() {
            if (_opToToken.Count > 0)
                return;

            //_opToToken[EOperation.Nop] = EPiToken.Nop;
            //_opToToken[EOperation.HasType] = EPiToken.HasType;
            _opToToken[EOperation.GarbageCollect] = EPiToken.GarbageCollect;
            _opToToken[EOperation.Plus] = EPiToken.Plus;
            _opToToken[EOperation.Minus] = EPiToken.Minus;
            _opToToken[EOperation.Multiply] = EPiToken.Multiply;
            _opToToken[EOperation.Divide] = EPiToken.Divide;
            //_opToToken[EOperation.Modulo] = EPiToken.Modulo;
            _opToToken[EOperation.Store] = EPiToken.Store;
            _opToToken[EOperation.Has] = EPiToken.Has;
            _opToToken[EOperation.Retrieve] = EPiToken.Retrieve;
            _opToToken[EOperation.Assign] = EPiToken.Assign;
            //_opToToken[EOperation.GetPath] = EPiToken.GetPath;
            _opToToken[EOperation.Suspend] = EPiToken.Suspend;
            _opToToken[EOperation.Resume] = EPiToken.Resume;
            _opToToken[EOperation.Replace] = EPiToken.Replace;
            _opToToken[EOperation.Assert] = EPiToken.Assert;
            _opToToken[EOperation.Write] = EPiToken.Write;
            _opToToken[EOperation.WriteLine] = EPiToken.WriteLine;
            _opToToken[EOperation.If] = EPiToken.If;
            _opToToken[EOperation.IfElse] = EPiToken.IfElse;
            //_opToToken[EOperation.StackToList] = EPiToken.StackToList;
            //_opToToken[EOperation.ListToStack] = EPiToken.ListToStack;
            _opToToken[EOperation.Depth] = EPiToken.Depth;
            _opToToken[EOperation.Dup] = EPiToken.Dup;
            _opToToken[EOperation.Clear] = EPiToken.Clear;
            _opToToken[EOperation.Swap] = EPiToken.Swap;
            _opToToken[EOperation.Break] = EPiToken.Break;
            _opToToken[EOperation.Rot] = EPiToken.Rot;
            //_opToToken[EOperation.Roll] = EPiToken.Roll;
            _opToToken[EOperation.RotN] = EPiToken.RotN;
            //_opToToken[EOperation.RollN] = EPiToken.RollN;
            _opToToken[EOperation.Pick] = EPiToken.Pick;
            _opToToken[EOperation.Over] = EPiToken.Over;
            _opToToken[EOperation.Freeze] = EPiToken.Freeze;
            _opToToken[EOperation.Thaw] = EPiToken.Thaw;
            //_opToToken[EOperation.FreezeText] = EPiToken.FreezeText;
            //_opToToken[EOperation.ThawText] = EPiToken.ThawText;
            //_opToToken[EOperation.FreezeYaml] = EPiToken.FreezeYaml;
            //_opToToken[EOperation.ThawYaml] = EPiToken.ThawYaml;
            _opToToken[EOperation.Not] = EPiToken.Not;
            _opToToken[EOperation.Equiv] = EPiToken.Equiv;
            _opToToken[EOperation.LogicalAnd] = EPiToken.And;
            _opToToken[EOperation.LogicalOr] = EPiToken.Or;
            _opToToken[EOperation.LogicalXor] = EPiToken.Xor;
            _opToToken[EOperation.Less] = EPiToken.Less;
            _opToToken[EOperation.Greater] = EPiToken.Greater;
            _opToToken[EOperation.GreaterOrEquiv] = EPiToken.GreaterEquiv;
            _opToToken[EOperation.LessOrEquiv] = EPiToken.LessEquiv;
            _opToToken[EOperation.NotEquiv] = EPiToken.NotEquiv;
            _opToToken[EOperation.Expand] = EPiToken.Expand;
            _opToToken[EOperation.ToArray] = EPiToken.ToArray;
            _opToToken[EOperation.ToMap] = EPiToken.ToMap;
            _opToToken[EOperation.ToSet] = EPiToken.ToSet;
            //_opToToken[EOperation.ToPair] = EPiToken.ToPair;
            _opToToken[EOperation.Size] = EPiToken.Size;
            _opToToken[EOperation.GetBack] = EPiToken.GetBack;
            _opToToken[EOperation.PushBack] = EPiToken.PushBack;
            _opToToken[EOperation.PushFront] = EPiToken.PushFront;
            _opToToken[EOperation.ToList] = EPiToken.ToList;
            _opToToken[EOperation.Remove] = EPiToken.Remove;
            _opToToken[EOperation.Insert] = EPiToken.Insert;
            _opToToken[EOperation.At] = EPiToken.At;
            _opToToken[EOperation.DebugPrintDataStack] = EPiToken.DebugPrintDataStack;
            _opToToken[EOperation.DebugPrintContextStack] = EPiToken.DebugPrintContextStack;
            _opToToken[EOperation.DebugPrint] = EPiToken.DebugPrint;
            _opToToken[EOperation.DebugPrintContinuation] = EPiToken.DebugPrintContinuation;
            //_opToToken[EOperation.DebugSetLevel] = EPiToken.DebugSetLevel;
            _opToToken[EOperation.SetFloatPrecision] = EPiToken.SetFloatPrecision;
            _opToToken[EOperation.Self] = EPiToken.Self;
            _opToToken[EOperation.GetMember] = EPiToken.GetMember;
            //_opToToken[EOperation.SetMember] = EPiToken.SetMember;
            //_opToToken[EOperation.SetMemberValue] = EPiToken.SetMemberValue;
            //_opToToken[EOperation.ForEachIn] = EPiToken.ForEachIn;
            //_opToToken[EOperation.ForLoop] = EPiToken.ForLoop;
            _opToToken[EOperation.Drop] = EPiToken.Drop;
            _opToToken[EOperation.DropN] = EPiToken.DropN;
        }
    }
}
