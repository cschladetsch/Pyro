namespace Pyro {
    /// <summary>
    ///     A part of a pathname, which is a sequence of PathElements
    /// </summary>
    public class PathElement {
        public string Ident;
        public Pathname.EElementType Type;

        public PathElement(Pathname.EElementType type = Pathname.EElementType.None) {
            Type = type;
        }

        public PathElement(string ident) {
            Type = Pathname.EElementType.Ident;
            Ident = ident;
        }
    }
}