using Diver.Exec;

namespace Diver.Language
{
    public interface ITranslator : IProcess
    {
        int TraceLevel { get; set; }
        bool Translate(string text, out Continuation result, EStructure st = EStructure.Program);
    }
}