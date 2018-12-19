using System;
using Diver.PiLang;
using NUnit.Framework;
using Debug = System.Diagnostics.Debug;

namespace Diver.Tests
{
    [TestFixture]
    public class TestPi
    {
        [Test]
        public void Test1()
        {
            var lex = new Lexer("1 2 +");
            Assert.IsTrue(lex.Process());
            Debug.WriteLine(lex.ToString());
        }
    }

}
