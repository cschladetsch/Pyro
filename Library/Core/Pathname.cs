﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pyro {
    /// <inheritdoc cref="IdentBase" />
    /// <summary>
    ///     A partial or fully-qualified path name.
    /// </summary>
    public class Pathname
        : IdentBase
            , IPathname {
        public enum EElementType {
            None
            , Separator
            , Ident
        }

        public const char Quote = '\'';
        public const char Slash = '/';

        public Pathname() {
            Elements = new List<Element>();
        }

        public Pathname(IList<Element> elements, bool quoted = false)
            : base(quoted) {
            Elements = elements;
        }

        public Pathname(string text) : base() {
            throw new NotImplementedException();
        }

        public IList<Element> Elements { get; }

        public string ToText(IRegistry reg = null) {
            throw new NotImplementedException();
        }

        public bool FromText(string s, IRegistry reg) {
            throw new NotImplementedException();
        }

        public bool FromText(IStringSlice s, IRegistry reg) {
            throw new NotImplementedException();
        }

        // public bool Quoted { get; set; }
        public new bool Quoted { get; set; }

        public bool IsAbsolute => throw new NotImplementedException();

        List<PathElement> IPathname.Elements => throw new NotImplementedException();

        public override string ToString() {
            var str = new StringBuilder();
            if (Quoted) {
                str.Append(Quote);
            }

            foreach (var elem in Elements)
                switch (elem.Type) {
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

            return str.ToString();
        }

        /// <summary>
        ///     A part of a Pathname
        /// </summary>
        public class Element {
            public string Ident;
            public EElementType Type;

            public Element(EElementType type = EElementType.None) {
                Type = type;
            }

            public Element(string ident) {
                Type = EElementType.Ident;
                Ident = ident;
            }
        }
    }
}