
using Pyro.Language.Impl;
using Pyro.Language.Tau.Lexer;

namespace Pyro.Language.Tau.Parser {
    
    public class TauParser 
        : ParserCommon<TauLexer, TauAstNode, TauToken, ETauToken, ETauAst, TauAstFactory>
        , IParser {
        public TauAstNode Result => _Stack.Peek();

        private readonly EStructure _structure;

        public TauParser(TauLexer lexer, IRegistry reg, EStructure st = EStructure.Namespace)
            : base(lexer, reg) {
            _Current = 0;
            _structure = st;
        }

        public bool Process() {
            if (_Lexer.Failed) {
                return Fail(_Lexer.Error);
            }

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
            if (Failed || !result) {
                return false;
            }

            if (!Maybe(ETauToken.Nop)) {
                return FailLocation("Unexpected extra stuff found");
            }

            return !Failed && _Stack.Count == 1 || InternalFail("Stack not empty after parsing");
        }

        private bool Definition() {
            while (!Failed && !Maybe(ETauToken.Nop)) {
                while (Namespace()) {
                }

                return Current().Type == ETauToken.Nop || FailLocation("Extra stuff found");
            }

            return true;
        }

        // TODO: push into ParserCommon
        private bool MaybeConsume(ETauToken type) {
            if (!Maybe(type)) {
                return false;
            }

            Consume();
            return true;
        }

        private bool Namespace() {
            if (!MaybeConsume(ETauToken.Namespace)) {
                return false;
            }

            var name = Expect(ETauToken.Identifier);
            if (Failed) {
                return Fail("Expected namespace name");
            }

            var @namespace = _AstFactory.New(ETauAst.Namespace, name.TauToken);
            _Stack.Push(@namespace);

            if (!MaybeConsume(ETauToken.OpenBrace)) {
                return FailLocation("Expected '{' after namespace name");
            }

            while (!Empty()) {
                while (MaybeConsume(ETauToken.Interface)) {
                    if (!Interface()) {
                        return false;
                    }
                }

                if (Failed) {
                    return false;
                }

                @namespace.Add(Pop());

                return MaybeConsume(ETauToken.CloseBrace) || FailLocation("Expected '}}' after namespace body");
            }

            return true;
        }

        private bool Interface() {
            _Stack.Push(_AstFactory.New(ETauAst.Interface, Expect(ETauToken.Identifier).TauToken));

            if (!MaybeConsume(ETauToken.OpenBrace)) {
                return FailLocation("Expected '{' after interface name");
            }

            while (true) {
                var next = Current();
                switch (next.Type) {
                    case ETauToken.String:
                    case ETauToken.Int:
                    case ETauToken.Float:
                    case ETauToken.Void:
                    case ETauToken.Identifier:
                        Consume();
                        if (!PropertyOrMethod(next)) {
                            return Fail("Property or method expected");
                        }
                        break;
                    case ETauToken.CloseBrace:
                        Consume();
                        return true;
                    default:
                        return FailLocation($@"Unexpected token {next.Type} in interface");
                }
            }

        }

        private bool PropertyOrMethod(TauToken type ) {
            var name = Expect(ETauToken.Identifier);
            if (MaybeConsume(ETauToken.OpenBrace)) {
                return Property(type, name);
            }

            if (MaybeConsume(ETauToken.OpenParan)) {
                return ParseMethod(type, name);
            }

            return FailLocation("Property or method expected");
        }

        private bool ParseMethod(TauToken returnType, TauAstNode name) {
            var method = _AstFactory.New(ETauAst.Method, name.TauToken);
            var parameters = _AstFactory.New(ETauAst.ParameterList);

            _AstFactory.AddChild(method, returnType);
            _AstFactory.AddChild(method, parameters);

            while (!Failed) {
                if (MaybeConsume(ETauToken.CloseParan)) {
                    break;
                }

                var type = ExpectTypeName();
                var argName = Expect(ETauToken.Identifier);

                parameters.Add(type);
                parameters.Add(argName);

                if (!MaybeConsume(ETauToken.Comma)) {
                    Expect(ETauToken.CloseParan);
                    break;
                }
            }

            Result.Add(method);
            return Require(ETauToken.Semi);
        }

        private TauAstNode ExpectTypeName() {
            var next = Current();
            switch (next.Type) {
                case ETauToken.String:
                case ETauToken.Int:
                case ETauToken.Float:
                case ETauToken.Identifier:
                    return _AstFactory.New(Consume());
            }

            FailLocation("Expected type name");
            return null;
        }

        private bool Property(TauToken type, TauAstNode name) {
            var property = _AstFactory.New(ETauAst.Property, name.TauToken);
            property.Add(type);
            if (MaybeConsume(ETauToken.Getter)) {
                if (!MaybeConsume(ETauToken.Semi)) {
                    return FailLocation("Expected ';' after getter");
                }

                property.Add(ETauToken.Getter);
            }

            if (MaybeConsume(ETauToken.Setter)) {
                if (!MaybeConsume(ETauToken.Semi)) {
                    return FailLocation("Expected ';' after setter");
                }

                property.Add(ETauToken.Setter);
            }

            if (!MaybeConsume(ETauToken.CloseBrace)) {
                return FailLocation("Expected '}' after property");
            }

            Result.Add(property);
            return true;
        }
    }
}