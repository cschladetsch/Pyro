using System;
using Diver.PiLang;
using NUnit.Framework;

namespace Diver.Tests
{
    [TestFixture]
    public class TestPi
    {
        [Test]
        public void Test1()
        {
            var factory = new TokenFactory();
            Lexer lex = new Lexer("1 2 +", factory);
        }
    }

}
