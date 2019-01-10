using System.Collections.Generic;
using Diver.Language.Impl;

namespace Diver.Language
{
    /// <summary>
    /// PiParser for the Pi language. It's quite simple.
    /// </summary>
    public class PiParser
        : ParserCommon<PiLexer, PiAstNode, PiToken, EPiToken, EPiAst, PiAstFactory>
        , IParser
    {
        public PiParser(PiLexer lexer) : base(lexer, null)
        {
        }

        public PiAstNode Root => _stack.Peek();

        public bool Process(PiLexer lex, EStructure structure = EStructure.None)
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
                    case EPiToken.Whitespace:
                    case EPiToken.Tab:
                    case EPiToken.NewLine:
                    case EPiToken.Comment:
                        continue;
                }

                _tokens.Add(tok);
            }
        }

        private bool Run(EStructure st)
        {
            _stack.Push(_astFactory.New(EPiAst.Continuation));
            while (!Failed && NextSingle(Top()))
                ;
            return !Failed;
        }

        private bool NextSingle(PiAstNode context)
        {
            if (Empty())
                return false;

            switch (Current().Type)
            {
                case EPiToken.Quote:
                case EPiToken.Separator:
                case EPiToken.Ident:
                    return ParsePathname(context);
                case EPiToken.OpenSquareBracket:
                    return ParseCompound(context, EPiAst.Array, EPiToken.CloseSquareBracket);
                case EPiToken.OpenBrace:
                    return ParseCompound(context, EPiAst.Continuation, EPiToken.CloseBrace);
                case EPiToken.CloseSquareBracket:
                case EPiToken.CloseBrace:
                    return FailLocation("Unopened compound");
                case EPiToken.None:
                    return false;
                // most pi tokens just fall through to being passed to translator
                default:
                    context.Add(AddValue(_astFactory.New(Consume())));
                    return true;
            }
        }

        private bool ParsePathname(PiAstNode context)
        {
            var elements = new List<Pathname.Element>();
            var prev = EPiToken.None;
            var quoted = false;
            while (true)
            {
                switch (Current().Type)
                {
                    case EPiToken.Quote:
                        if (quoted || elements.Count > 0)
                            goto done;
                        quoted = true;
                        break;
                    case EPiToken.Separator:
                        if (prev == EPiToken.Separator)
                            return FailLocation("Malformed pathname");
                        elements.Add(new Pathname.Element(Pathname.EElementType.Separator));
                        break;
                    case EPiToken.Ident:
                        // we can have an ident after an optional initial quote, or after a separator
                        var start = prev == EPiToken.None || prev == EPiToken.Quote;
                        if (start || prev == EPiToken.Separator)
                            elements.Add(new Pathname.Element(Current().Text));
                        else
                            goto done;
                        break;
                    default:
                        goto done;
                }

                prev = Current().Type;
                Consume();
                if (Empty())
                    break;
            }

        done:
            PiAstNode node = null;
            if (elements.Count == 1 && elements[0].Type == Pathname.EElementType.Ident)
            {
                node = NewNode(EPiAst.Ident);
                node.Value = new Label(elements[0].Ident, quoted);
            }
            else
            {
                node = NewNode(EPiAst.Pathname);
                node.Value = new Pathname(elements, quoted);
            }
            context.Add(node);

            return true;
        }

        private static PiAstNode AddValue(PiAstNode node)
        {
            var token = node.PiToken;
            var text = token.GetText();
            switch (token.Type)
            {
                case EPiToken.Int:
                    node.Value = int.Parse(text);
                    break;
                case EPiToken.String:
                    node.Value = text;
                    break;
                default:
                    node.Value = text;
                    break;
            }

            return node;
        }

        private bool ParseCompound(PiAstNode root, EPiAst type, EPiToken end)
        {
            Consume();
            var node = NewNode(type);
            while (!Empty() && !Try(end))
            {
                if (!NextSingle(node))
                    return FailLocation($"Malformed compound {type}");

                if (Failed)
                    return false;
            }

            if (Empty())
                return FailLocation($"Malformed compound {type}");

            Consume();
            root.Add(node);
            return true;
        }
    }
}
