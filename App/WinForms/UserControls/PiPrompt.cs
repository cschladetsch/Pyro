namespace WinForms.UserControls {
    using System.Windows.Forms;
    
    using Pyro.Language;

    public partial class PiPrompt 
        : UserControlBase {
        
        private readonly int MaxHistoryCount = 20;

        public PiPrompt() {
            InitializeComponent();
        }

        private void Process() {
            Process(comboBox1.Text);
            comboBox1.Text = "";
        }

        private void Process(string text) {
            if (string.IsNullOrEmpty(text.Trim())) {
                return;
            }
            MainForm.Run(text, ELanguage.Pi);
            AddToHistory(text);
        }

        private void AddToHistory(string text) {
            while (comboBox1.Items.Count > MaxHistoryCount) {
                comboBox1.Items.RemoveAt(0);
            }

            comboBox1.Items.Add(text);
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case '\r':
                    Process();
                    e.Handled = true;
                    break;
            }
        }
    }
}