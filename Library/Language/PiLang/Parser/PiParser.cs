using System.Collections.Generic;

namespace Diver.Language
{
    /// <summary>
    /// PiParser for the Pi language. It's quite simple.
    /// </summary>
    public class PiParser
        : ParserCommon<PiLexer, PiAstNode, PiToken, EPiToken, EPiAst, PiAstFactory>
    {
        public PiParser(PiLexer lexer) : base(lexer, null)
        {
        }

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
            _root = _astFactory.New(EPiAst.Continuation);
            while (!Failed && NextSingle(_root))
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
                    return Fail(_lexer.CreateErrorMessage(Current(), "%s", "Unopened compound"));
                case EPiToken.None:
                    return false;
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
                        if (quoted || prev != EPiToken.None)
                            return FailLocation("Malformed pathname");
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
                        if (start ^ prev != EPiToken.Separator)
                            return FailLocation("Malformed pathname");
                        elements.Add(new Pathname.Element(Current().Text));
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

        bool FailLocation(string fmt, params object[] args)
        {
            return Fail(_lexer.CreateErrorMessage(Current(), fmt, args));
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
