using System;
using System.Diagnostics;
using System.Windows.Forms;
using Pyro.Language;

namespace WinForms.UserControls {
    public partial class PiPrompt 
        : UserControlBase {
        
        public PiPrompt() {
            InitializeComponent();
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e) {
            var box = (ComboBox)sender;
            var text = box.Text;
            //ColorisePi()
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            var box = (ComboBox)sender;
            box.Text = box.Items[box.SelectedIndex].ToString();
        }

        private void Process() {
            Process(comboBox1.Text);
            comboBox1.Text = "";
        }

        private void Process(string text) {
            MainForm.Run(text, ELanguage.Pi);
            while (comboBox1.Items.Count > 20) {
                comboBox1.Items.RemoveAt(0);
            }
            comboBox1.Items.Add(text);
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case '\r':
                    Process();
                    break;
            }
        }
    }
}