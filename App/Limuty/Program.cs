using System;
using System.Linq;
using System.Text;
using System.Threading;
using Pyro.AppCommon;
using Pyro.ExecutionContext;
using static System.Console;
using Con = System.Console;

namespace Pyro.Limuty
{
    class Program
        : AppCommonBase
    {
        private static void Main(string[] args)
        {
            new Program(args).Run();
        }

        public Program(object[] args)
            : base(args)
        {
            _ctx = new Context();
        }

        private void Run()
        {
            try
            {
                while (true)
                {
                    WritePrompt();
                    _ctx.Exec(ReadLine());
                    WriteDataStack();
                }

            }
            catch (Exception e)
            {
                Error(e.Message);
            }
        }

        private void WritePrompt()
        {
            Write("Limuty> ", ConsoleColor.Gray);
            ForegroundColor = ConsoleColor.White;
        }

        public void WriteDataStack()
        {
            ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            var results = _ctx.Executor.DataStack.ToList();
            results.Reverse();
            var n = 0;
            foreach (var result in results)
                str.AppendLine($"{n++}: {_ctx.Registry.ToText(result)}");
            Write(str.ToString());
        }

        protected override void Shutdown()
        {
            WriteLine("Ctrl-C", ConsoleColor.DarkGray);
            Exit();
        }

        private Context _ctx;
    }
}
