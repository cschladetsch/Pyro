namespace Pyro.Test
{
     using System.Collections.Generic;
    using NUnit.Framework;
    using Language.Lexer;

    [TestFixture]
    public class TestNativeObjects
        : TestCommon
    {
        public class UserModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }

            public string GetFoo()
            {
                return "Foo";
            }

            public string GetString()
            {
                return $"{Name}:{Email}:{Age}";
            }
        }

        [Test]
        public void TestNew()
        {
            AssertSameTokens("new", EPiToken.New);
            _Registry.Register(new ClassBuilder<UserModel>(_Registry)
                .Class);

            PiRun(@"'UserModel new");
            var user = Pop<UserModel>();
            Assert.IsNotNull(user);

            PiRun(@"'UserModel new 'u # 'GetFoo u .@ &");
            AssertPop("Foo");
        }

        [Test]
        public void TestList()
        {
            PiRun("[] 'a # 42 'Add a .@ & a");
            var list = Pop<List<object>>();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(42, (int)list[0]);
        }

        [Test]
        public void TestString()
        {
            AssertSameTokens("#", EPiToken.Store);

            const string pre0 = "\"foobar\" 's # ";
            const string length = pre0 + "'Length s .@";
            const string sub0 = pre0 + "0 3 'Substring s .@ &";
            const string sub1 = pre0 + "3 3 'Substring s .@ &";
            const string sub2 = pre0 + "4 'Substring1 s .@ &";

            AssertSameTokens("s", EPiToken.Ident);
            AssertSameTokens(pre0, EPiToken.String, EPiToken.Quote, EPiToken.Ident, EPiToken.Store);
            AssertSameTokens(sub0, EPiToken.String, EPiToken.Quote, EPiToken.Ident, EPiToken.Store, EPiToken.Int,
                EPiToken.Int, EPiToken.Quote, EPiToken.Ident, EPiToken.Ident, EPiToken.GetMember, EPiToken.Suspend);

            PiRun(sub0);
            AssertPop("foo");

            PiRun(length);
            AssertPop(6);

            PiRun(sub1);
            AssertPop("bar");

            PiRun(sub2);
            AssertPop("ar");
        }
    }
}

