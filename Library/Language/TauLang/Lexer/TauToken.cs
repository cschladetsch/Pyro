using Pyro.Language.Impl;

namespace Pyro.Language.Tau.Lexer {
    public class TauToken
        : TokenBase<ETauToken>
        , ITokenNode<ETauToken> {
        
        public TauToken() 
            : base() {
            _type = ETauToken.Nop;
        }

        public TauToken(ETauToken type, Slice slice)
            : base(type, slice) {
        }

        public override string ToString() {
            if (Slice.Length == 0 || string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(Text.Trim())) {
                return Type.ToString();
            }

            return $"{Type}: '{Text}'";
        }

        public override bool Equals(object obj) {
            if (obj is TauToken tok) {
                return _type == tok._type && Equals(Slice, tok.Slice);
            }

            return false;
        }

        protected bool Equals(TauToken other) {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode() {
            return (int)_type % Slice.GetHashCode();
        }
    }
}
