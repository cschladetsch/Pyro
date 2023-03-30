namespace Pyro.Language.Tau {
    public enum EGeneratedType {
        ENone,
        EProxy,
        EAgent,
    }

    public static class EGeneratedTypeExtensions {
        public static string GetGeneratedTypeString(this EGeneratedType eGeneratedType) {
            switch (eGeneratedType) {
                case EGeneratedType.EProxy:
                    return "Proxy";
                case EGeneratedType.EAgent:
                    return "Agent";
                default:
                    return "None";
            }
        }
    }
}