using System;
using System.Text;
using System.Collections.Generic;

namespace Pyro
{
    /// <inheritdoc />
    /// <summary>
    /// A partial or fully-qualified path name.
    /// </summary>
    public class Pathname
        : IdentBase
        , IPathname
    {
        public enum EElementType
        {
            None,
            Separator,
            Ident
        }

        public const char Quote = '\'';
        public const char Slash = '/';

        /// <summary>
        /// A part of a Pathname
        /// </summary>
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

        public IList<Element> Elements { get; }

        public Pathname()
            => Elements = new List<Element>();

        public Pathname(IList<Element> elements, bool quoted = false)
            : base(quoted)
        {
            Elements = elements;
        }

        public Pathname(string text) : base(false)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            if (Quoted)
                str.Append(Quote);

            foreach (var elem in Elements)
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

        public string ToText(IRegistry reg = null)
        {
            throw new NotImplementedException();
        }

        public bool FromText(string s, IRegistry reg)
        {
            throw new NotImplementedException();
        }

        public bool FromText(IStringSlice s, IRegistry reg)
        {
            throw new NotImplementedException();
        }

        public bool Quoted { get; set; }
    }
}

