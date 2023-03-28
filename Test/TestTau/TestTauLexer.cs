using NUnit.Framework;
using Pyro.Language.Tau.Lexer;

namespace TestTau {
    public class TestTauLexer
        : TestTauCommon {
        [Test]
        public void Test1() {
            const string input = 
@"namespace Foo {
    interface Bar {
        event Func<int, string> SomeEvent;
        string Name { get; }
        float Age { get; set; }
        int Sum(int a, int b);
        void Call();
    }
}";
            AssertSameTokens(input, 
                ETauToken.Namespace, ETauToken.Identifier, ETauToken.OpenBrace, 
                    ETauToken.Interface, ETauToken.Identifier, ETauToken.OpenBrace,
                        ETauToken.Event, ETauToken.Func, ETauToken.LessThan, ETauToken.Int, ETauToken.Comma, ETauToken.String, ETauToken.GreaterThan, ETauToken.Identifier, ETauToken.Semi,
                        ETauToken.String, ETauToken.Identifier, ETauToken.OpenBrace, ETauToken.Getter, ETauToken.Semi, ETauToken.CloseBrace,
                        ETauToken.Float, ETauToken.Identifier, ETauToken.OpenBrace, ETauToken.Getter, ETauToken.Semi, ETauToken.Setter, ETauToken.Semi, ETauToken.CloseBrace,
                        ETauToken.Int, ETauToken.Identifier, ETauToken.OpenParan, ETauToken.Int, ETauToken.Identifier, ETauToken.Comma, ETauToken.Int, ETauToken.Identifier, ETauToken.CloseParan, ETauToken.Semi, 
                        ETauToken.Void, ETauToken.Identifier, ETauToken.OpenParan, ETauToken.CloseParan, ETauToken.Semi, 
                    ETauToken.CloseBrace,
                ETauToken.CloseBrace,
                ETauToken.Nop
                );
        }
    }
}