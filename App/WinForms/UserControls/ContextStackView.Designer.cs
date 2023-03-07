namespace WinForms.UserControls {
    public partial class ContextStackView {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContextStackView));
            this.fromRho = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.contextStack = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.label3 = new System.Windows.Forms.Label();
            this.stackView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.label4 = new System.Windows.Forms.Label();
            this.codeView = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fromRhoEditor = new System.Windows.Forms.ToolStripButton();
            this.nextOperation = new System.Windows.Forms.ToolStripButton();
            this.fromRho.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fromRho
            // 
            this.fromRho.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.fromRho.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromRhoEditor,
            this.nextOperation});
            this.fromRho.Location = new System.Drawing.Point(0, 0);
            this.fromRho.Name = "fromRho";
            this.fromRho.Size = new System.Drawing.Size(441, 39);
            this.fromRho.TabIndex = 0;
            this.fromRho.Text = "toolStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 592);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(353, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(49, 20);
            this.toolStripStatusLabel1.Text = "Status";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Context";
            // 
            // contextStack
            // 
            this.contextStack.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.contextStack.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextStack.HideSelection = false;
            this.contextStack.Location = new System.Drawing.Point(0, 78);
            this.contextStack.Name = "contextStack";
            this.contextStack.Size = new System.Drawing.Size(353, 169);
            this.contextStack.TabIndex = 3;
            this.contextStack.UseCompatibleStateImageBehavior = false;
            this.contextStack.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 34;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Loc";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Code";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 289);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(353, 13);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 305);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "Scope";
            // 
            // stackView
            // 
            this.stackView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.stackView.Dock = System.Windows.Forms.DockStyle.Top;
            this.stackView.HideSelection = false;
            this.stackView.Location = new System.Drawing.Point(0, 334);
            this.stackView.Name = "stackView";
            this.stackView.Size = new System.Drawing.Size(353, 156);
            this.stackView.TabIndex = 7;
            this.stackView.UseCompatibleStateImageBehavior = false;
            this.stackView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Name";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Value";
            this.columnHeader5.Width = 96;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Type";
            this.columnHeader6.Width = 131;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Id";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 529);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(353, 13);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(0, 545);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 23);
            this.label4.TabIndex = 9;
            this.label4.Text = "Code";
            // 
            // codeView
            // 
            this.codeView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10});
            this.codeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeView.HideSelection = false;
            this.codeView.Location = new System.Drawing.Point(0, 574);
            this.codeView.Name = "codeView";
            this.codeView.Size = new System.Drawing.Size(441, 166);
            this.codeView.TabIndex = 10;
            this.codeView.UseCompatibleStateImageBehavior = false;
            this.codeView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "#";
            this.columnHeader8.Width = 42;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Object";
            this.columnHeader9.Width = 208;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Id";
            this.columnHeader10.Width = 100;
            // 
            // fromRhoEditor
            // 
            this.fromRhoEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fromRhoEditor.Image = ((System.Drawing.Image)(resources.GetObject("fromRhoEditor.Image")));
            this.fromRhoEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fromRhoEditor.Name = "fromRhoEditor";
            this.fromRhoEditor.Size = new System.Drawing.Size(29, 36);
            this.fromRhoEditor.Text = "toolStripButton2";
            // 
            // nextOperation
            // 
            this.nextOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextOperation.Image = ((System.Drawing.Image)(resources.GetObject("nextOperation.Image")));
            this.nextOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextOperation.Name = "nextOperation";
            this.nextOperation.Size = new System.Drawing.Size(29, 24);
            this.nextOperation.Text = "toolStripButton1";
            this.nextOperation.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // ContextStackView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.codeView);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.stackView);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.contextStack);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.fromRho);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ContextStackView";
            this.Size = new System.Drawing.Size(353, 618);
            this.Load += new System.EventHandler(this.ContextStackView_Load);
            this.fromRho.ResumeLayout(false);
            this.fromRho.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip fromRho;
        private System.Windows.Forms.ToolStripButton nextOperation;
        private System.Windows.Forms.ToolStripButton fromRhoEditor;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView contextStack;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView stackView;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView codeView;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
    }
}
