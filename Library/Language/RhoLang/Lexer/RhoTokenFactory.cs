using Diver.Language;

namespace Diver.RhoLang
{
    public class RhoTokenFactory : ITokenFactory<ERhoToken, RhoToken>
    {
        public RhoToken NewToken(ERhoToken en, Slice slice)
        {
            throw new System.NotImplementedException();
        }

        public RhoToken NewTokenIdent(Slice slice)
        {
            throw new System.NotImplementedException();
        }

        public RhoToken NewTokenString(Slice slice)
        {
            throw new System.NotImplementedException();
        }

        public RhoToken NewEmptyToken(Slice slice)
        {
            throw new System.NotImplementedException();
        }

        public void SetLexer(LexerBase lexer)
        {
            throw new System.NotImplementedException();
        }
    }
}
