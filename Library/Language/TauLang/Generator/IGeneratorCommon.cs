using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public interface IGeneratorCommon {
        TauParser Parser { get; }

        string Result { get; }

        bool Run();

        bool GenerateNamespace(TauAstNode node);

        bool GenerateInterface(TauAstNode @interface);

        bool GenerateProperty(TauAstNode property);

        bool GenerateMethod(TauAstNode member);

        bool FailLocation(TauAstNode node, string text);

        bool Fail(string err);
    }
}