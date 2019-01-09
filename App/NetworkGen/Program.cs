using System;
using System.Reflection;

namespace NetworkGen
{
    enum EBuildType
    {
        Proxy,
        Agent,
    }

    class AssemblyProcess
    {
        protected static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger(); 

        public AssemblyProcess(string asmName)
        {
            var asm = Assembly.LoadFile(asmName);
            foreach (var module in asm.GetModules())
            {
                foreach (var type in module.GetTypes())
                {
                    _logger.Info(type.FullName);
                }
            }
        }
    }

    class BuildAgent : AssemblyProcess
    {
        public BuildAgent(string asmName) : base(asmName)
        {
        }
    }

    class BuildProxy : AssemblyProcess
    {
        public BuildProxy(string asmName) : base(asmName)
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var type = Enum.Parse(typeof(EBuildType), args[0]);
            var inputAsm = args[1];
            var outputDir = args[2];

            switch (type)
            {
                case EBuildType.Agent:
                    BuildAgent(inputAsm, outputDir);
                    break;
                case EBuildType.Proxy:
                    BuildProxy(inputAsm, outputDir);
                    break;
            }
        }

        private static void BuildProxy(string inputAsm, string outputDir)
        {
            throw new NotImplementedException();
        }

        private static void BuildAgent(string inputAsm, string outputDir)
        {
            throw new NotImplementedException();
        }
    }
}
