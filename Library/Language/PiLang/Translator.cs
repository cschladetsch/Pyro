using System;
using System.Collections.Generic;
using Diver.Exec;

namespace Diver.Language.PiLang
{
    /// <summary>
    /// Translates input Pi text source code to an executable Continuation
    /// </summary>
    public class Translator : ProcessCommon
    {
        public IRef<Continuation> Continuation;
        public Lexer Lexer => _lexer;
        public Parser Parser => _parser;

        public Translator(IRegistry reg, string input) : base(reg)
        {
            Run(input);
        }

        public bool Run(string input)
        {
            _lexer = new Lexer(input);
            
            if (!_lexer.Process())
                return Fail($"LexerError: {_lexer.Error}");

            _parser = new Parser(_lexer);
            if (!_parser.Process(_lexer, EStructure.Sequence))
                return Fail($"ParserError: {_parser.Error}");

            Continuation = New(new Continuation(new List<object>()));

            return TranslateNode(_parser.Root, Continuation.Value.Code);
        }

        public override string ToString()
        {
            return $"=== Translator:\nInput: {_lexer.Input}Lexer: {_lexer}\nParser: {_parser}";
        }

        private bool TranslateNode(AstNode node, IList<object> objects)
        {
            if (node == null)
                return Fail("Null Ast Node");

            foreach (var ast in node.Children)
                if (!AddNode(ast, objects))
                    return false;

            return true;
        }

        private bool AddNode(AstNode ast, IList<object> objects)
        {
            switch (ast.Type)
            {
                case EAst.None:
                    break;
                case EAst.TokenType:
                    AddToken(ast, objects);
                    break;
                case EAst.Array:
                    return TranslateArray(ast, objects);
                case EAst.Map:
                    return TranslateMap(ast, objects);
                case EAst.Continuation:
                    return TranslateContinuation(ast, objects);
                default:
                    objects.Add(ast.Value);
                    break;
            }

            return true;
        }

        private void AddToken(AstNode node, IList<object> objects)
        {
            var token = node.Token;
            switch (token.Type)
            {
                case EToken.Plus:
                    objects.Add(New(EOperation.Plus));
                    break;
                case EToken.Minus:
                    objects.Add(New(EOperation.Minus));
                    break;
                case EToken.Store:
                    objects.Add(New(EOperation.Store));
                    break;
                case EToken.Retrieve:
                    objects.Add(New(EOperation.Retrieve));
                    break;
                case EToken.Dup:
                    objects.Add(New(EOperation.Dup));
                    break;
                case EToken.Clear:
                    objects.Add(New(EOperation.Clear));
                    break;
                case EToken.Swap:
                    objects.Add(New(EOperation.Swap));
                    break;
                default:
                    objects.Add(node.Value);
                    break;
            }
        }

        private bool TranslateMap(AstNode ast, IList<object> objects)
        {
            throw new NotImplementedException("TranslateMap");
        }

        private bool TranslateArray(AstNode astNode, IList<object> objects)
        {
            var array = new List<object>();
            if (!TranslateNode(astNode, array))
                return Fail($"Failed to translate ${astNode}");
            objects.Add(array);
            return true;
        }

        private bool TranslateContinuation(AstNode astNode, IList<object> objects)
        {
            var cont = New(new Continuation(new List<object>()));
            if (!TranslateNode(astNode, cont.Value.Code))
                return Fail($"Failed to translate ${astNode}");
            objects.Add(cont);
            return true;
        }

        private Lexer _lexer;
        protected internal Parser _parser;
    }
}
