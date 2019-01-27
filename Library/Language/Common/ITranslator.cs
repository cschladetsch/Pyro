using Pyro.Exec;

namespace Pyro.Language
{
    public interface ITranslator : IProcess
    {
        int TraceLevel { get; set; }
        bool Translate(string text, out Continuation result, EStructure st = EStructure.Program);
    }
}