using Diver.RhoLang;

namespace Diver.Language
{
    public class RhoLexer 
        : LexerCommon<ERhoToken, RhoToken, RhoTokenFactory>
    {
        public RhoLexer(string input) : base(input)
        {
        }
    }
}
