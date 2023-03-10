using Pyro.Language;
using Pyro.RhoLang.Lexer;
using System;
using System.Drawing;

namespace WinForms {
    /// <inheritdoc />
    /// <summary>
    /// Colorise Rho script.
    /// </summary>
    public partial class MainForm {
        private void ColoriseRho() {
            CheckFonts();

            var rtb = _rhoInput;
            BeginUpdate(rtb);

            var input = rtb.Text + " \n";
            var lex = new RhoLexer(input);
            try {
                DefaultColors(lex, input);

                lex.Process();

                foreach (var tok in lex.Tokens)
                    ColoriseRhoToken(tok, tok.Slice);
            } catch (Exception e) {
                output1.Append($"{e.Message}: {lex.Error}");
            } finally {
                EndUpdate(rtb);
            }
        }

        private void DefaultColors(LexerBase lex, string input) {
            SetRhoSliceColor(new Slice(lex, 0, input.Length - 1), Color.Black, _defaultFont);
        }

        private bool ColoriseRhoToken(RhoToken tok, Slice slice) {
            if (slice.Length < 0)
                return false;

            bool Render(Color color) => SetRhoSliceColor(slice, color, _defaultFont);
            bool RenderBold(Color color) => SetRhoSliceColor(slice, color, _boldFont);

            switch (tok.Type) {
                case ERhoToken.Assert:
                    return RenderBold(Color.DarkMagenta);

                case ERhoToken.Fun:
                    return RenderBold(Color.Black);

                case ERhoToken.Class:
                    return RenderBold(Color.DarkSlateGray);

                // operators
                case ERhoToken.Plus:
                case ERhoToken.Minus:
                case ERhoToken.Multiply:
                case ERhoToken.Divide:
                //case ERhoToken.Modulo:
                case ERhoToken.And:
                case ERhoToken.Or:
                case ERhoToken.Xor:
                case ERhoToken.Equiv:
                case ERhoToken.GreaterEquiv:
                case ERhoToken.Less:
                case ERhoToken.LessEquiv:
                    return Render(Color.SteelBlue);

                case ERhoToken.String:
                    // color the quotes too
                    var expanded = new Slice(slice.Lexer, slice.LineNumber, slice.Start - 1, slice.End + 1);
                    return SetRhoSliceColor(expanded, Color.Blue, _boldFont);

                case ERhoToken.Int:
                    return Render(Color.Brown);

                case ERhoToken.Float:
                    return Render(Color.Chocolate);

                case ERhoToken.Ident:
                    return RenderBold(Color.DarkSlateBlue);

                case ERhoToken.Quote:
                    return Render(Color.Gray);

                case ERhoToken.New:
                case ERhoToken.Delete:
                    return Render(Color.DarkOliveGreen);

                case ERhoToken.Comment:
                    return Render(Color.Gray);

                default:
                    return Render(DefaultForeColor);
            }
        }

        private bool SetRhoSliceColor(Slice slice, Color color, Font font)
            => SetSliceColor(_rhoInput, slice, color, font);
    }
}
