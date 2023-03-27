
using Pyro.Language.Impl;
using Pyro.Language.Tau.Lexer;

namespace Pyro.Language.Tau.Parser {
    
    public class TauParser 
        : ParserCommon<TauLexer, TauAstNode, TauToken, ETauToken, ETauAst, TauAstFactory>
        , IParser {
        private TauAstNode Result => _Stack.Peek();

        private readonly EStructure _structure;

        public TauParser(TauLexer lexer, IRegistry reg, EStructure st = EStructure.Namespace)
            : base(lexer, reg) {
            _Current = 0;
            _structure = st;
        }

        public bool Process() {
            if (_Lexer.Failed)
                return Fail(_Lexer.Error);

            RemoveWhitespace();

            return Parse();
        }
        
        private void RemoveWhitespace() {
            foreach (var tok in _Lexer.Tokens) {
                switch (tok.Type) {
                    case ETauToken.Tab:
                    case ETauToken.WhiteSpace:
                    case ETauToken.Comment:
                    case ETauToken.NewLine:
                        continue;
                }

                _Tokens.Add(tok);
            }
        }

        private bool Parse() {
            var result = Definition();
            if (Failed || !result)
                return false;

            if (!Maybe(ETauToken.Nop))
                return FailLocation("Unexpected extra stuff found");

            return _Stack.Count == 1 || InternalFail("Semantic stack not empty after parsing");
        }

        private bool Definition() {
            while (!Failed && !Maybe(ETauToken.Nop)) {
                while (Namespace());

                return Current().Type == ETauToken.Nop || FailLocation("Extra stuff found");
            }

            return true;
        }

        private bool Namespace() {
            if (!Maybe(ETauToken.Namespace))
                return false;

            var name = Expect(ETauToken.Identifier);
            if (Failed) {
                return Fail("Expected namespace name");
            }

            var ns = _AstFactory.New(ETauAst.Namespace, name.TauToken);
            _Stack.Push(ns);

            if (!Maybe(ETauToken.OpenBrace)) {
                return FailLocation("Expected '{' after namespace name");
            }

            while (true) {
                while (Interface()) {
                }

                if (Failed) {
                    return false;
                }

                if (!Maybe(ETauToken.CloseBrace)) {
                    return FailLocation("Expected '}' after namespace body");
                }
            }
        }

        private bool Interface() {
            if (!Maybe(ETauToken.Interface)) {
                return FailLocation("Expected interface");
            }

            Consume();

            Result.Add(_AstFactory.New(ETauAst.Interface, Expect(ETauToken.Identifier).TauToken));

            if (!Maybe(ETauToken.OpenBrace)) {
                return FailLocation("Expected '{' after interface name");
            }

            while (true) {
                var next = Current();
                switch (next.Type) {
                    case ETauToken.String:
                    case ETauToken.Int:
                    case ETauToken.Float:
                    case ETauToken.Identifier:
                        if (!PropertyOrMethod(next)) {
                            return Fail("Property or method expected");
                        }
                        break;
                    case ETauToken.CloseBrace:
                        return true;
                    default:
                        return FailLocation(@"Unexpected token {next} in interface");
                }
            }
        }

        private bool PropertyOrMethod(TauToken type ) {
            var name = Expect(ETauToken.Identifier);
            if (Maybe(ETauToken.OpenBrace)) {
                Consume();
                return Property(type, name);
            }

            if (Maybe(ETauToken.OpenParan)) {
                Consume();
                return ParseMethod(type, name);
            }

            return FailLocation("Property or method expected");
        }

        private bool ParseMethod(TauToken returnType, TauAstNode name) {
            var method = _AstFactory.New(ETauAst.Method, name.TauToken);
            var parameters = _AstFactory.New(ETauAst.ParameterList);

            _AstFactory.AddChild(method, returnType);
            _AstFactory.AddChild(method, parameters);

            while (!Failed && !Maybe(ETauToken.CloseParan)) {
                if (Maybe(ETauToken.CloseParan)) {
                    break;
                }

                var type = Consume();
                var argName = Expect(ETauToken.Identifier);

                parameters.Add(_AstFactory.New(ETauAst.Ident, type));
                parameters.Add(argName);

                if (!Maybe(ETauToken.Comma)) {
                    return Maybe(ETauToken.CloseParan);
                }
            }

            Result.Add(method);
            return true;
        }

        private bool Property(TauToken tauToken, TauAstNode name) {
            var property = _AstFactory.New(ETauAst.Property, name.TauToken);
            property.Add(name);
            if (Maybe(ETauToken.Getter)) {
                Consume();
                if (!Maybe(ETauToken.Semi)) {
                    return FailLocation("Expected ';' after getter");
                }

                property.Add(ETauToken.Getter);
            }

            if (Maybe(ETauToken.Setter)) {
                Consume();
                if (!Maybe(ETauToken.Semi)) {
                    return FailLocation("Expected ';' after setter");
                }

                property.Add(ETauToken.Setter);
            }

            if (!Maybe(ETauToken.CloseBrace)) {
                return FailLocation("Expected '}' after property");
            }

            Result.Add(property);
            return true;
        }
    }
}