using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Unity3dScenePersistenceTool
{
    class Scene
    {

    }

    class Program
    {
        static void Main(string[] args)
        {
            var scenePath = @"C:\Users\chris\work\TemplateUnityProject\NewProject\Assets\Scenes\Main.unity";
            var contents = System.IO.File.ReadAllText(scenePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            var scene = deserializer.Deserialize<object>(contents);
        }
    }
}
