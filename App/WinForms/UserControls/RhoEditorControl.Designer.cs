namespace WinForms.UserControls {
    partial class RhoEditorControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RhoEditorControl));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toPiEditor = new System.Windows.Forms.ToolStripButton();
            this.toRhoEditor = new System.Windows.Forms.ToolStripButton();
            this.toTauEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.runButton = new System.Windows.Forms.ToolStripButton();
            this.loadButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toPiButton = new System.Windows.Forms.ToolStripButton();
            this.RichTextBoxPi = new System.Windows.Forms.RichTextBox();
            this.RichTextBoxRho = new System.Windows.Forms.RichTextBox();
            this.RichTextBoxTau = new System.Windows.Forms.RichTextBox();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.toolStripStatusLabel1 });
            this.statusStrip1.Location = new System.Drawing.Point(0, 542);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(547, 34);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(97, 29);
            this.toolStripStatusLabel1.Text = "Insert";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("MesloLGS NF", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.toPiEditor, this.toRhoEditor, this.toTauEditor, this.toolStripSeparator1, this.runButton, this.loadButton, this.toolStripSeparator2, this.toPiButton });
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(547, 28);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toPiEditor
            // 
            this.toPiEditor.Image = ((System.Drawing.Image)(resources.GetObject("toPiEditor.Image")));
            this.toPiEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toPiEditor.Name = "toPiEditor";
            this.toPiEditor.Size = new System.Drawing.Size(50, 25);
            this.toPiEditor.Text = "Pi";
            this.toPiEditor.Click += new System.EventHandler(this.toPiEditor_Click);
            // 
            // toRhoEditor
            // 
            this.toRhoEditor.Image = ((System.Drawing.Image)(resources.GetObject("toRhoEditor.Image")));
            this.toRhoEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toRhoEditor.Name = "toRhoEditor";
            this.toRhoEditor.Size = new System.Drawing.Size(60, 25);
            this.toRhoEditor.Text = "Rho";
            this.toRhoEditor.Click += new System.EventHandler(this.toRhoEditor_Click);
            // 
            // toTauEditor
            // 
            this.toTauEditor.Image = ((System.Drawing.Image)(resources.GetObject("toTauEditor.Image")));
            this.toTauEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toTauEditor.Name = "toTauEditor";
            this.toTauEditor.Size = new System.Drawing.Size(60, 25);
            this.toTauEditor.Text = "Tau";
            this.toTauEditor.Click += new System.EventHandler(this.toTauEditor_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // runButton
            // 
            this.runButton.Image = ((System.Drawing.Image)(resources.GetObject("runButton.Image")));
            this.runButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(60, 25);
            this.runButton.Text = "Run";
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Image = ((System.Drawing.Image)(resources.GetObject("loadButton.Image")));
            this.loadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(70, 25);
            this.loadButton.Text = "Load";
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // toPiButton
            // 
            this.toPiButton.Image = ((System.Drawing.Image)(resources.GetObject("toPiButton.Image")));
            this.toPiButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toPiButton.Name = "toPiButton";
            this.toPiButton.Size = new System.Drawing.Size(70, 25);
            this.toPiButton.Text = "toPi";
            this.toPiButton.Click += new System.EventHandler(this.toPiButton_Click);
            // 
            // RichTextBoxPi
            // 
            this.RichTextBoxPi.Font = new System.Drawing.Font("Source Code Pro", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextBoxPi.Location = new System.Drawing.Point(3, 259);
            this.RichTextBoxPi.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.RichTextBoxPi.Name = "RichTextBoxPi";
            this.RichTextBoxPi.Size = new System.Drawing.Size(343, 261);
            this.RichTextBoxPi.TabIndex = 4;
            this.RichTextBoxPi.Text = "Rho Text";
            // 
            // RichTextBoxRho
            // 
            this.RichTextBoxRho.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextBoxRho.Location = new System.Drawing.Point(61, 113);
            this.RichTextBoxRho.Name = "RichTextBoxRho";
            this.RichTextBoxRho.Size = new System.Drawing.Size(100, 96);
            this.RichTextBoxRho.TabIndex = 5;
            this.RichTextBoxRho.Text = "Pi Text";
            // 
            // RichTextBoxTau
            // 
            this.RichTextBoxTau.Location = new System.Drawing.Point(184, 73);
            this.RichTextBoxTau.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.RichTextBoxTau.Name = "RichTextBoxTau";
            this.RichTextBoxTau.Size = new System.Drawing.Size(361, 430);
            this.RichTextBoxTau.TabIndex = 3;
            this.RichTextBoxTau.Text = "TauText";
            // 
            // RhoEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RichTextBoxPi);
            this.Controls.Add(this.RichTextBoxRho);
            this.Controls.Add(this.RichTextBoxTau);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "RhoEditorControl";
            this.Size = new System.Drawing.Size(547, 576);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

        private System.Windows.Forms.ToolStripButton toTauEditor;

        private System.Windows.Forms.ToolStripButton toRhoEditor;
        private System.Windows.Forms.ToolStripButton toPiEditor;

        private System.Windows.Forms.ToolStripButton toPiButton;

        private System.Windows.Forms.ToolStripButton loadButton;

        private System.Windows.Forms.ToolStripButton runButton;

        private System.Windows.Forms.RichTextBox RichTextBoxPi;
        private System.Windows.Forms.RichTextBox RichTextBoxRho;
        private System.Windows.Forms.RichTextBox RichTextBoxTau;

        private System.Windows.Forms.ToolStrip toolStrip1;

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}
