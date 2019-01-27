namespace Pryo
{
    /// <summary>
    /// A part of a pathname, which is a sequence of PathElements
    /// </summary>
    public class PathElement
    {
        public Pathname.EElementType Type;
        public string Ident;

        public PathElement(Pathname.EElementType type = Pathname.EElementType.None)
        {
            Type = type;
        }

        public PathElement(string ident)
        {
            Type = Pathname.EElementType.Ident;
            Ident = ident;
        }
    }
}