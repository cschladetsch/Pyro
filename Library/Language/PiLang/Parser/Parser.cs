﻿using System;

namespace Diver.Language.PiLang
{
    /// <summary>
    /// Parser for the Pi language. It's quite simple.
    /// </summary>
    public class Parser : ParserCommon<Lexer, AstNode, Token, EToken, EAst, AstFactory>
    {

        public Parser(LexerBase lexer) : base(lexer, null)
        {
        }

        public override bool Process(Lexer lex, EStructure structure = EStructure.None)
        {
            _current = 0;
            _indent = 0;
            _lexer = lex;

            if (_lexer.Failed)
                return Fail(_lexer.Error);

            RemoveWhitespace();

            return Run(structure);
        }

        private void RemoveWhitespace()
        {
            foreach (var tok in _lexer.Tokens)
            {
                switch (tok.Type)
                {
                    case EToken.Whitespace:
                    case EToken.Tab:
                    case EToken.NewLine:
                    case EToken.Comment:
                        continue;
                }

                _tokens.Add(tok);
            }
        }

        private bool Run(EStructure st)
        {
            _root = _astFactory.New(EAst.Continuation);
            while (!Failed && NextSingle(_root))
                ;
            return !Failed;
        }

        private bool NextSingle(AstNode context)
        {
            if (Empty())
                return false;

            switch (Current().Type)
            {
                case EToken.OpenSquareBracket:
                    return ParseCompound(context, EAst.Array, EToken.CloseSquareBracket);
                case EToken.OpenBrace:
                    return ParseCompound(context, EAst.Continuation, EToken.CloseBrace);
                case EToken.CloseSquareBracket:
                case EToken.CloseBrace:
                    Fail(_lexer.CreateErrorMessage(Current(), "%s", "Unopened compound"));
                    return false;
                case EToken.None:
                    return false;
                default:
                    context.Add(AddValue(_astFactory.New(Consume())));
                    return true;
            }
        }

        private static AstNode AddValue(AstNode node)
        {
            var token = node.Token;
            var text = token.GetText();
            switch (token.Type)
            {
                case EToken.Int:
                    node.Value = int.Parse(text);
                    break;
                case EToken.String:
                    node.Value = text;
                    break;
                default:
                    node.Value = text;
                    break;
            }

            return node;
        }

        private bool ParseCompound(AstNode root, EAst type, EToken end)
        {
            Consume();
            var node = NewNode(type);
            while (!Empty() && !Failed && !Try(end))
            {
                if (!NextSingle(node))
                    return Fail(_lexer.CreateErrorMessage(Current(), "Malformed compound %s", type.ToString()));
            }

            if (Empty())
                return Fail("Malformed compound");

            if (Failed)
                return false;

            Consume();
            root.Add(node);
            return true;
        }
    }
}
