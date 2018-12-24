using Diver.Language;
using Diver.RhoLang.Lexer;

namespace Diver.RhoLang
{
    public class RhoToken : ITokenNode<ERhoToken>
    {
        public ERhoToken Type { get; }
    }
}
