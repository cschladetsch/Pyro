using Diver.Exec;

namespace Diver.Language
{
    public interface ITranslator : IProcess
    {
        Continuation Result { get; }
        bool Translate(string text, EStructure st = EStructure.Program);
        void Reset();
    }
}