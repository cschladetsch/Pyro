using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Pyro.Language;
using Pyro.Language.Lexer;

namespace WinForms
{
    partial class MainForm
    {
        private Font _defaultFont;
        private Font _boldFont;

        private bool ColorisePi()
        {
            if (_defaultFont == null)
            {
                _defaultFont = piInput.Font;
                _boldFont = new Font(_defaultFont.FontFamily, _defaultFont.Size, FontStyle.Bold);
            }

            var rtb = piInput;
            BeginUpdate(rtb);

            var input = piInput.Text + " \n";
            var lex = new PiLexer(input);
            try
            {
                Colorise(new Slice(lex, 0, input.Length - 1), Color.Black, _defaultFont);

                lex.Process();
                Console.WriteLine(lex.Error);

                foreach (var tok in lex.Tokens)
                    ColorisePiToken(tok, tok.Slice);
            }
            catch (Exception e)
            {
                output.Text = $"{e.Message}: {lex.Error}";
            }
            finally
            {
                EndUpdate(rtb);
            }

            return true;
        }

        private bool ColorisePiToken(PiToken tok, Slice slice)
        {
            if (slice.Length < 0)
                return false;

            bool Render(Color color) => Colorise(slice, color, _defaultFont);
            bool RenderBold(Color color) => Colorise(slice, color, _boldFont);

            switch (tok.Type)
            {
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
                    return Colorise(expanded, Color.Blue, _boldFont);

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

        private bool Colorise(Slice slice, Color color, Font font)
        {
            if (slice.Length < 0)
                return false;

            var rtb = piInput;
            var scroll = rtb.AutoScrollOffset;
            var slct = rtb.SelectionIndent;
            var ss = rtb.SelectionStart;

            rtb.SelectionStart = GetStartOffset(rtb, slice);
            rtb.SelectionLength = slice.Length;
            rtb.SelectionColor = color;
            rtb.SelectionFont = font;

            rtb.SelectionStart = ss;
            rtb.SelectionIndent = slct;
            rtb.AutoScrollOffset = scroll;

            return true;
        }

        private static int GetStartOffset(Control rtb, Slice slice)
        {
            var lines = rtb.Text.Split('\n');
            var offset = 0;
            for (var n = 0; n < Math.Min(lines.Length, slice.LineNumber); ++n)
                offset += lines[n].Length + 1;

            return offset + slice.Start;
        }

        #region Native
        private const int WmUser = 0x0400;
        private const int EmSeteventmask = (WmUser + 69);
        private const int WmSetredraw = 0x0b;
        private IntPtr _oldEventMask;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public void BeginUpdate(Control control)
        {
            SendMessage(control.Handle, WmSetredraw, IntPtr.Zero, IntPtr.Zero);
            _oldEventMask = SendMessage(control.Handle, EmSeteventmask, IntPtr.Zero, IntPtr.Zero);
        }

        public void EndUpdate(Control control)
        {
            SendMessage(control.Handle, WmSetredraw, (IntPtr)1, IntPtr.Zero);
            SendMessage(control.Handle, EmSeteventmask, IntPtr.Zero, _oldEventMask);
        }
        #endregion
    }
}

