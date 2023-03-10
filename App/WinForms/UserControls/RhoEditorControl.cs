using System.Data;

namespace WinForms.UserControls {
    using System;
    using System.Windows.Forms;
    
    using Pyro.Language;

    public partial class RhoEditorControl
        : UserControlBase {

        public ELanguage Language {
            get => _language;
            set => SetLanguage(value);
        }

        public RichTextBox RichTextBox {
            get {
                switch (Language) {
                case ELanguage.None:
                    break;
                case ELanguage.Pi:
                    return RichTextBoxPi;
                case ELanguage.Rho:
                    return RichTextBoxRho;
                case ELanguage.Tau:
                    return RichTextBoxTau;
                default:
                    throw new ArgumentOutOfRangeException();
                }

                return null;
            }
        }

        private ELanguage _language;

        public RhoEditorControl() {
            InitializeComponent();

            RichTextBoxPi.Text = RichTextBoxRho.Text = RichTextBoxTau.Text = "";
            RichTextBoxPi.Dock = RichTextBoxRho.Dock = RichTextBoxTau.Dock = DockStyle.Fill;
            
            RichTextBoxRho.Multiline = true;
            RichTextBoxRho.AcceptsTab = true;
            
            RichTextBoxTau.Multiline = true;
            RichTextBoxTau.AcceptsTab = true;
            
            RichTextBoxPi.Multiline = true;

            Language = ELanguage.Rho;
        }

        public RichTextBox GetLanguageText()
            => GetLanguageText(_language);
        
        public RichTextBox GetLanguageText(ELanguage language) {
            switch (language) {
            case ELanguage.None:
                break;
            case ELanguage.Pi:
                return RichTextBoxPi;
            case ELanguage.Rho:
                return RichTextBoxRho;
            case ELanguage.Tau:
                return RichTextBoxTau;
            }
            throw new ArgumentOutOfRangeException();
        }

        private void SetLanguage(ELanguage lang) {
            switch (_language = lang) {
            case ELanguage.None:
                break;
            case ELanguage.Pi:
                SelectEditor(RichTextBoxPi);
                break;
            case ELanguage.Rho:
                SelectEditor(RichTextBoxRho);
                break;
            case ELanguage.Tau:
                SelectEditor(RichTextBoxTau);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lang), lang, null);
            }
        }

        private void SelectEditor(Control richTextBox) {
            RichTextBox.TextChanged -= UpdateStatus;
            RichTextBox.CursorChanged -= UpdateStatus;
            RichTextBoxPi.Hide();
            RichTextBoxRho.Hide();
            RichTextBoxTau.Hide();
            
            richTextBox.Show();
            RichTextBox.TextChanged += UpdateStatus;
            RichTextBox.CursorChanged += UpdateStatus;
            UpdateStatusText();
        }

        private void UpdateStatus(object sender, EventArgs e) {
            UpdateStatusText();
        }

        private void UpdateStatusText() {
            var rtb = RichTextBox;
            var index = rtb.SelectionStart;
            var line = rtb.GetLineFromCharIndex(index);

            // Get the column.
            var firstChar = rtb.GetFirstCharIndexFromLine(line);
            var column = index - firstChar;
            toolStripStatusLabel1.Text = $"{_language.ToString()}: {line}:{column}";
        }

        private void ExecutePi() {
            var rtb = GetLanguageText();
            var text = rtb.Lines[rtb.GetLineFromCharIndex(rtb.SelectionStart)];
            MainForm.Run(text, Language);
        }
        
        private void Execute() {
            var script = RichTextBox.SelectedText.Length > 0 ? RichTextBox.SelectedText : RichTextBox.Text;
            MainForm.Run(script, Language);
        }
        private void PiInputKeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Enter: {
                        if (e.Control) {
                            MainForm.RunCurrent();
                            e.Handled = true;
                        }
                        break;
                    }
            }
        }

        private void toPiEditor_Click(object sender, EventArgs e) {
            Language = ELanguage.Pi;
        }

        private void toRhoEditor_Click(object sender, EventArgs e) {
            Language = ELanguage.Rho;
        }

        private void toTauEditor_Click(object sender, EventArgs e) {
            Language = ELanguage.Tau;
        }

        private void loadButton_Click(object sender, EventArgs e) {
            MainForm.Decompile();
        }

        private void toPiButton_Click(object sender, EventArgs e) {
            MainForm.ConvertToPi();
            Language = ELanguage.Pi;
        }

        private void runButton_Click(object sender, EventArgs e) {
            MainForm.RunCurrent();
        }
    }
}