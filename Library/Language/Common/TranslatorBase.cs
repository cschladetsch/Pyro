using Diver.Exec;

namespace Diver.Language
{
    public class TranslatorBase<TParser> : TranslatorCommon
    {
        protected TranslatorBase(IRegistry reg) : base(reg)
        {
        }

        protected override Continuation Translate(
            string text, EStructure st = EStructure.Statement)
        {
            throw new System.NotImplementedException();
        }
    }
}