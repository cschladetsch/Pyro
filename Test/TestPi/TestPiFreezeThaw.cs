namespace Pyro.Test
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class TestPiFreezeThaw
        : TestCommon
    {
        [SetUp]
        public new void Setup()
        {
            _Exec.Rethrows = true;
        }

        [Test]
        public void TestList()
        {
            PiRun("[1 2 3]");
            var list = Pop<List<object>>();
            var list2 = FullCircle(list);
            Assert.AreEqual(list, list2);
        }

        [Test]
        public void TestDict()
        {
            var makeMap = "1 2 \"foo\" \"bar\" 2 tomap";
            PiRun(makeMap);
            var dict = Pop<Dictionary<object, object>>();
            var dict2 = FullCircle(dict);
            foreach (var kv in dict)
            {
                Assert.IsTrue(dict2.ContainsKey(kv.Key));
                Assert.IsTrue(dict2.TryGetValue(kv.Key, out var val));
                Assert.AreEqual(val, kv.Value);
            }
        }

        public class User
        {
            public string Name;
            public string Last;
            public int Age;
            public IRef<Organisation> Org;
        }

        public class Organisation
        {
            public string Email;
            public IList<IRef<User>> Users;
        }

        [Test]
        public void TestPersistedInstances()
        {
            var user = _Registry.Add<User>().Value;
            user.Name = "Freddy";
            user.Last = "Blogs";
            user.Age = 42;

            var pi = _Registry.ToPiScript(user);
            PiRun(pi);
            var user2 = Pop<User>();
            Assert.AreEqual(user.Name, user2.Name);
            Assert.AreEqual(user.Last, user2.Last);
            Assert.AreEqual(user.Age, user2.Age);
        }

        [Test]
        public void TestPersistentReferencedObjects()
        {
            var org = _Registry.Add<Organisation>();
            org.Value.Email = "foo@org.com";
            var user = _Registry.Add<User>();
            user.Value.Name = "Freddy";
            user.Value.Org = org;

            var pi = _Registry.ToPiScript(user.Value);
            PiRun(pi);

            var user2 = Pop<User>();
            Assert.AreEqual("Freddy", user2.Name);
            Assert.IsNotNull(user2.Org);
            Assert.AreEqual("foo@org.com", user2.Org.Value.Email);
        }

        [Test]
        public void TestFreezeThaw()
        {
            TestFreezeThawPi("true assert");

            TestFreezeThawPi("1 2 +");
            AssertPop(3);

            TestFreezeThawPi("{1 2 +} &");
            AssertPop(3);

            TestFreezeThawPi(@"""foo"" ""bar"" +");
            AssertPop("foobar");

            TestFreezeThawPi("[1 2 3]");
            var list = Pop<List<object>>();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);

            FreezeThaw("Boolean.pi");
            FreezeThaw("Comments.pi");
            FreezeThaw("Arithmetic.pi");
            FreezeThaw("Array.pi");
            FreezeThaw("Conditionals.pi");
            FreezeThaw("Continuations.pi");
            FreezeThaw("Strings.pi");
        }

        private T FullCircle<T>(T obj)
        {
            var pi = _Registry.ToPiScript(obj);
            PiRun(pi);
            return Pop<T>();
        }
    }
}

