using Diver.RhoLang;

namespace Diver.Language
{
    public class RhoLexer : ILexerCommon<RhoToken>
    {
        public string CreateErrorMessage(RhoToken tok, string fmt, params object[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}
