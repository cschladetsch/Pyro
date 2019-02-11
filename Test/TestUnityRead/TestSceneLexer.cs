using Diver.Test;
using NUnit.Framework;

namespace Pyro.Tests
{
    [TestFixture]
    public class TestUnity3dScene : TestCommon
    {
        //[Test]
        public void TestLoad()
        {
            const string scenePathname = @"Examples\Scene.unity";
            var local = MakeLocalPath(scenePathname);
            var text = System.IO.File.ReadAllText(local);
            Assert.IsNotEmpty(text, $"Couldn't read {local}");
            var lexer = new Unity3d.Scene.UnityLexer(text);
            Assert.IsTrue(lexer.Process(), lexer.Error);
        }
    }
}