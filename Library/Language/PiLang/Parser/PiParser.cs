using System.Collections.Generic;

namespace Diver.Language.PiLang
{
    /// <summary>
    /// _parser for the Pi language. It's quite simple.
    /// </summary>
    public class PiParser : ParserCommon<PiLexer, AstNode, PiToken, EToken, EAst, AstFactory>
    {
        public PiParser(LexerBase lexer) : base(lexer, null)
        {
        }

        public override bool Process(PiLexer lex, EStructure structure = EStructure.None)
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
                case EToken.Quote:
                case EToken.Separator:
                case EToken.Ident:
                    return ParsePathname(context);

                case EToken.OpenSquareBracket:
                    return ParseCompound(context, EAst.Array, EToken.CloseSquareBracket);
                case EToken.OpenBrace:
                    return ParseCompound(context, EAst.Continuation, EToken.CloseBrace);
                case EToken.CloseSquareBracket:
                case EToken.CloseBrace:
                    return Fail(_lexer.CreateErrorMessage(Current(), "%s", "Unopened compound"));
                case EToken.None:
                    return false;
                default:
                    context.Add(AddValue(_astFactory.New(Consume())));
                    return true;
            }
        }

        private bool ParsePathname(AstNode context)
        {
            var elements = new List<Pathname.Element>();
            var prev = EToken.None;
            var quoted = false;
            while (true)
            {
                switch (Current().Type)
                {
                    case EToken.Quote:
                        if (quoted || prev != EToken.None)
                            return FailLocation("Malformed pathname");
                        quoted = true;
                        break;
                    case EToken.Separator:
                        if (prev == EToken.Separator)
                            return FailLocation("Malformed pathname");
                        elements.Add(new Pathname.Element(Pathname.EElementType.Separator));
                        break;
                    case EToken.Ident:
                        // we can have an ident after an optional initial quote, or after a separator
                        var start = prev == EToken.None || prev == EToken.Quote;
                        if (start ^ prev != EToken.Separator)
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
            AstNode node = null;
            if (elements.Count == 1 && elements[0].Type == Pathname.EElementType.Ident)
            {
                node = NewNode(EAst.Ident);
                node.Value = new Label(elements[0].Ident, quoted);
            }
            else
            {
                node = NewNode(EAst.Pathname);
                node.Value = new Pathname(elements, quoted);
            }
            context.Add(node);

            return true;
        }

        bool FailLocation(string fmt, params object[] args)
        {
            return Fail(_lexer.CreateErrorMessage(Current(), fmt, args));
        }

        private static AstNode AddValue(AstNode node)
        {
            var token = node.PiToken;
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
