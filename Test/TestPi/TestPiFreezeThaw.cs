using System.Collections.Generic;

using NUnit.Framework;

namespace Pyro.Test
{
    [TestFixture]
    public class TestPiFreezeThaw
        : TestCommon
    {
        [SetUp]
        public new void Setup()
        {
            _exec.Rethrows = true;
        }

        [Test]
        public void TestList()
        {
            PiRun("[1 2 3]");
            var list = Pop<List<object>>();
            var list2 = ObjectToPiToObject(list);
            Assert.AreEqual(list, list2);
        }

        [Test]
        public void TestDict()
        {
            var makeMap = "1 2 \"foo\" \"bar\" 2 tomap";
            PiRun(makeMap);
            var dict = Pop<Dictionary<object, object>>();
            var dict2 = ObjectToPiToObject(dict);
            foreach (var kv in dict)
            {
                Assert.IsTrue(dict2.ContainsKey(kv.Key));
                Assert.IsTrue(dict2.TryGetValue(kv.Key, out var val));
                Assert.AreEqual(val, kv.Value);
            }
        }

        public class User : Reflected<User>
        {
            public string Name;
            public string Last;
            public int Age;
            public IConstRef<Organisation> Org;
        }

        public class Organisation : Reflected<Organisation>
        {
            public string Email;
        }

        [Test]
        public void TestPersistedInstances()
        {
            var user = _reg.Add<User>().Value;
            user.Name = "Freddy";
            user.Last = "Blogs";
            user.Age = 42;

            var pi = _reg.ToPiScript(user);
            PiRun(pi);
            var user2 = Pop<User>();
            Assert.AreEqual(user.Name, user2.Name);
            Assert.AreEqual(user.Last, user2.Last);
            Assert.AreEqual(user.Age, user2.Age);
        }

        [Test]
        public void TestPersistentReferencedObjects()
        {
            var org = _reg.Add<Organisation>();
            org.Value.Email = "foo@org.com";
            var user = _reg.Add<User>();
            user.Value.Name = "Freddy";
            user.Value.Org = org;

            var pi = _reg.ToPiScript(user.Value);
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

            TestFreezeThawScript("Boolean.pi");
            TestFreezeThawScript("Comments.pi");
            TestFreezeThawScript("Arithmetic.pi");
            //TestFreezeThawScript("Array.pi");
            //TestFreezeThawScript("Conditionals.pi");
            TestFreezeThawScript("Continuations.pi");
            TestFreezeThawScript("Strings.pi");
        }

        private T ObjectToPiToObject<T>(T obj)
        {
            var pi = _reg.ToPiScript(obj);
            PiRun(pi);
            return Pop<T>();
        }

    }
}

