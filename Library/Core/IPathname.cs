namespace Pyro {

    using System.Collections.Generic;

    public class PathElement {
        public enum EType {
            None,
            Separator,
            Identifier,
        }

        public EType ElementType { get; set; }
        public IIdentifer Identifer { get; }

        public PathElement(IIdentifer identifer) {
            ElementType = EType.Identifier;
            Identifer = identifer;
        }

        public PathElement(EType type) {
            ElementType = type;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// DOC
    /// </summary>
    public interface IPathname
        : IIdentifer {

        bool IsAbsolute { get; }

        List<PathElement> Elements { get; }
    }
}
