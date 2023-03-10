using Pyro.Language;
using Pyro.Language.Lexer;
using System;
using System.Drawing;
using System.Windows.Forms;
using WinForms.UserControls;

namespace WinForms {
    /// <summary>
    /// Colorise Pi script.
    /// </summary>
    partial class MainForm {
        
        private bool ColorisePi() {
            CheckFonts();

            var rtb = _editor.GetLanguageText(ELanguage.Pi);
            BeginUpdate(rtb);

            var input = rtb.Text + " \n";
            var lex = new PiLexer(input);
            try {
                // start by setting everything to default color and font
                SetPiSliceColor(new Slice(lex, 0, input.Length - 1), Color.Black, _defaultFont);

                lex.Process();

                foreach (var tok in lex.Tokens)
                    ColorisePiToken(tok, tok.Slice);
            } catch (Exception e) {
                output.Text += $"{e.Message}: {lex.Error}";
            } finally {
                EndUpdate(rtb);
            }

            return true;
        }

        private bool ColorisePiToken(PiToken tok, Slice slice) {
            if (slice.Length < 0)
                return false;

            bool Render(Color color) => SetPiSliceColor(slice, color, _defaultFont);
            bool RenderBold(Color color) => SetPiSliceColor(slice, color, _boldFont);

            switch (tok.Type) {
                case EPiToken.Assert:
                    return RenderBold(Color.DarkMagenta);

                // operators
                case EPiToken.Plus:
                case EPiToken.Minus:
                case EPiToken.Multiply:
                case EPiToken.Divide:
                //case EPiToken.Modulo:
                case EPiToken.And:
                case EPiToken.Or:
                case EPiToken.Xor:
                case EPiToken.Equiv:
                case EPiToken.GreaterEquiv:
                case EPiToken.Less:
                case EPiToken.LessEquiv:
                    return Render(Color.SteelBlue);

                case EPiToken.String:
                    // color the quotes too
                    var expanded = new Slice(slice.Lexer, slice.LineNumber, slice.Start - 1, slice.End + 1);
                    return SetSliceColor(_piInput, expanded, Color.Blue, _boldFont);

                case EPiToken.Int:
                    return Render(Color.Brown);

                case EPiToken.Float:
                    return Render(Color.Chocolate);

                case EPiToken.Ident:
                    return RenderBold(Color.DarkSlateBlue);

                case EPiToken.Quote:
                    return Render(Color.Gray);

                case EPiToken.New:
                case EPiToken.Delete:
                case EPiToken.Swap:
                case EPiToken.Drop:
                case EPiToken.DropN:
                case EPiToken.Depth:
                case EPiToken.Clear:
                case EPiToken.Rot:
                case EPiToken.RotN:
                case EPiToken.Expand:
                case EPiToken.Size:
                case EPiToken.Dup:
                case EPiToken.GetType:
                    return Render(Color.DarkOliveGreen);

                case EPiToken.Comment:
                    return Render(Color.Gray);

                case EPiToken.ToList:
                case EPiToken.ToArray:
                case EPiToken.ToMap:
                case EPiToken.ToSet:
                    return Render(Color.DarkGreen);

                default:
                    return Render(DefaultForeColor);
            }
        }

        private bool SetPiSliceColor(Slice slice, Color color, Font font)
            => SetSliceColor(_piInput, slice, color, font);
    }
}

