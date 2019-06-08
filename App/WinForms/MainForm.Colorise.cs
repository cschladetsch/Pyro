using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Pyro.Language;

namespace WinForms
{
    public partial class MainForm
    {
        private static int GetStartOffset(Control rtb, Slice slice)
        {
            var lines = rtb.Text.Split('\n');
            var offset = 0;
            for (var n = 0; n < Math.Min(lines.Length, slice.LineNumber); ++n)
                offset += lines[n].Length + 1;

            return offset + slice.Start;
        }

        private bool SetSliceColor(RichTextBox rtb, Slice slice, Color color, Font font)
        {
            if (slice.Length < 0)
                return false;

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

        private void CheckFonts()
        {
            if (_defaultFont != null)
                return;

            _defaultFont = piInput.Font;
            _boldFont = new Font(_defaultFont.FontFamily, _defaultFont.Size, FontStyle.Bold);
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
