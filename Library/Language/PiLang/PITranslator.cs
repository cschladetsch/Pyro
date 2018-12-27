using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Diver.Exec;

namespace Diver.Language
{
    /// <summary>
    /// Translates input Pi text source code to an executable Continuation
    /// </summary>
    public class PiTranslator : ProcessCommon
    {
        public IRef<Continuation> Continuation => _continuation;
        public PiLexer Lexer => _lexer;
        public PiParser Parser => _parser;

        public PiTranslator(IRegistry reg, string input) : base(reg)
        {
            Run(input);
        }

        public bool Run(string input)
        {
            _lexer = new PiLexer(input);
            
            if (!_lexer.Process())
                return Fail($"LexerError: {_lexer.Error}");

            _parser = new PiParser(_lexer);
            if (!Parser.Process(_lexer, EStructure.Sequence))
                return Fail($"ParserError: {Parser.Error}");

            _continuation = New(new Continuation(new List<object>()));

            return TranslateNode(Parser.Root, Continuation.Value.Code);
        }

        public override string ToString()
        {
            return $"=== PITranslator:\nInput: {_lexer.Input}PiLexer: {_lexer}\nParser: {Parser}";
        }

        private bool TranslateNode(PiAstNode node, IList<object> objects)
        {
            return node?.Children.All(ast => AddNode(ast, objects))
               ?? Fail("Null Ast Node");
        }

        private bool AddNode(PiAstNode piAst, IList<object> objects)
        {
            switch (piAst.Type)
            {
                case EPiAst.None:
                    break;
                case EPiAst.TokenType:
                    AddToken(piAst, objects);
                    break;
                case EPiAst.Array:
                    return TranslateArray(piAst, objects);
                case EPiAst.Map:
                    return TranslateMap(piAst, objects);
                case EPiAst.Continuation:
                    return TranslateContinuation(piAst, objects);
                default:
                    objects.Add(piAst.Value);
                    break;
            }

            return true;
        }

        private void AddToken(PiAstNode node, IList<object> objects)
        {
            var token = node.PiToken;
            switch (token.Type)
            {
                case EPiToken.Plus:
                    objects.Add(EOperation.Plus);
                    break;
                case EPiToken.Minus:
                    objects.Add(EOperation.Minus);
                    break;
                case EPiToken.Multiply:
                    objects.Add(EOperation.Multiply);
                    break;
                case EPiToken.Divide:
                    objects.Add(EOperation.Divide);
                    break;
                case EPiToken.Store:
                    objects.Add(EOperation.Store);
                    break;
                case EPiToken.Retrieve:
                    objects.Add(EOperation.Retrieve);
                    break;
                case EPiToken.Dup:
                    objects.Add(EOperation.Dup);
                    break;
                case EPiToken.Clear:
                    objects.Add(EOperation.Clear);
                    break;
                case EPiToken.Swap:
                    objects.Add(EOperation.Swap);
                    break;
                case EPiToken.Break:
                    objects.Add(EOperation.Break);
                    break;
                case EPiToken.Assert:
                    objects.Add(EOperation.Assert);
                    break;
                case EPiToken.True:
                    objects.Add(true);
                    break;
                case EPiToken.Not:
                    objects.Add(EOperation.Not);
                    break;
                case EPiToken.False:
                    objects.Add(false);
                    break;
                case EPiToken.Equiv:
                    objects.Add(EOperation.Equiv);
                    break;
                case EPiToken.And:
                    objects.Add(EOperation.LogicalAnd);
                    break;
                case EPiToken.Or:
                    objects.Add(EOperation.LogicalOr);
                    break;
                case EPiToken.Xor:
                    objects.Add(EOperation.LogicalXor);
                    break;
                case EPiToken.Size:
                    objects.Add(EOperation.Size);
                    break;
                case EPiToken.ToList:
                    objects.Add(EOperation.ToList);
                    break;
                case EPiToken.ToArray:
                    objects.Add(EOperation.ToArray);
                    break;
                case EPiToken.ToMap:
                    objects.Add(EOperation.ToMap);
                    break;
                case EPiToken.ToSet:
                    objects.Add(EOperation.ToSet);
                    break;
                case EPiToken.Expand:
                    objects.Add(EOperation.Expand);
                    break;
                case EPiToken.Insert:
                    objects.Add(EOperation.Insert);
                    break;
                case EPiToken.Remove:
                    objects.Add(EOperation.Remove);
                    break;
                case EPiToken.PushFront:
                    objects.Add(EOperation.PushFront);
                    break;
                case EPiToken.PushBack:
                    objects.Add(EOperation.PushBack);
                    break;
                case EPiToken.GetBack:
                    objects.Add(EOperation.GetBack);
                    break;
                case EPiToken.Has:
                    objects.Add(EOperation.Has);
                    break;
                case EPiToken.At:
                    objects.Add(EOperation.At);
                    break;
                case EPiToken.DebugPrintDataStack:
                    objects.Add(EOperation.DebugPrintDataStack);
                    break;
                case EPiToken.DebugPrintContinuation:
                    objects.Add(EOperation.DebugPrintContinuation);
                    break;
                case EPiToken.DebugPrintContextStack:
                    objects.Add(EOperation.DebugPrintContextStack);
                    break;
                case EPiToken.DebugPrint:
                    objects.Add(EOperation.DebugPrint);
                    break;
                default:
                    objects.Add(node.Value);
                    break;
            }
        }

        private bool TranslateMap(PiAstNode piAst, IList<object> objects)
        {
            throw new NotImplementedException("TranslateMap");
        }

        private bool TranslateArray(PiAstNode piAstNode, IList<object> objects)
        {
            var array = new List<object>();
            if (!TranslateNode(piAstNode, array))
                return Fail($"Failed to translate ${piAstNode}");
            objects.Add(array);
            return true;
        }

        private bool TranslateContinuation(PiAstNode piAstNode, IList<object> objects)
        {
            var cont = New(new Continuation(new List<object>()));
            if (!TranslateNode(piAstNode, cont.Value.Code))
                return Fail($"Failed to translate ${piAstNode}");
            objects.Add(cont);
            return true;
        }

        private PiLexer _lexer;
        protected internal PiParser _parser;
        private IRef<Continuation> _continuation;
    }
}
