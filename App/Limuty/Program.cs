using Pyro.AppCommon;

namespace Pyro.Limuty
{
    class Program : AppCommonBase
    {
        static void Main(string[] args)
        {
            new Program(args).Run();
        }

        public Program(params object[] args)
            : base(args)
        {
        }

        private void Run()
        {
            WriteLine("...doing unity things...");
        }

        protected override void Shutdown()
        {
        }
    }
}
