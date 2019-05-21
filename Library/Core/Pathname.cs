using System;
using System.Collections.Generic;
using System.Text;

namespace Pyro
{
    /// <summary>
    /// A partial or fully-qualified path name.
    /// </summary>
    public class Pathname : IdentBase
    {
        public enum EElementType
        {
            None,
            Separator,
            Ident
        }

        public const char Quote = '\'';
        public const char Slash = '/';

        public class Element
        {
            public EElementType Type;
            public string Ident;

            public Element(EElementType type = EElementType.None)
            {
                Type = type;
            }

            public Element(string ident)
            {
                Type = EElementType.Ident;
                Ident = ident;
            }
        }

        public IList<Element> Elements => _elements;

        public Pathname()
        {
            _elements = new List<Element>();
        }

        public Pathname(IList<Element> elements, bool quoted = false)
            : base(quoted)
        {
            _elements = elements;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            if (Quoted)
                str.Append(Quote);
            foreach (var elem in _elements)
            {
                switch (elem.Type)
                {
                    case EElementType.None:
                        break;
                    case EElementType.Separator:
                        str.Append(Slash);
                        break;
                    case EElementType.Ident:
                        str.Append(elem.Ident);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return str.ToString();
        }

        private readonly IList<Element> _elements;
    }
}
