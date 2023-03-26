using NUnit.Framework;
using Pyro.Language.Tau.Lexer;
namespace TestTau {


    public class Tests
        : TestTauCommon {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void Test1() {
            var input =
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
                ETauToken.Namespace, ETauToken.Ident, ETauToken.OpenBrace, 
                    ETauToken.Interface, ETauToken.Ident, ETauToken.OpenBrace,
                        ETauToken.Event, ETauToken.Func, ETauToken.LessThan, ETauToken.Int, ETauToken.Comma, ETauToken.String, ETauToken.GreaterThan, ETauToken.Ident, ETauToken.Semi,
                        ETauToken.String, ETauToken.Ident, ETauToken.OpenBrace, ETauToken.Getter, ETauToken.Semi, ETauToken.CloseBrace,
                        ETauToken.Float, ETauToken.Ident, ETauToken.OpenBrace, ETauToken.Getter, ETauToken.Semi, ETauToken.Setter, ETauToken.Semi, ETauToken.CloseBrace,
                        ETauToken.Int, ETauToken.Ident, ETauToken.OpenParan, ETauToken.Int, ETauToken.Ident, ETauToken.Comma, ETauToken.Int, ETauToken.Ident, ETauToken.CloseParan, ETauToken.Semi, 
                        ETauToken.Void, ETauToken.Ident, ETauToken.OpenParan, ETauToken.CloseParan, ETauToken.Semi, 
                    ETauToken.CloseBrace,
                ETauToken.CloseBrace,
                ETauToken.Nop
                );
        }
    }
}