namespace Main
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeLabelingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heckbertLabelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContentPanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.printDialog = new System.Windows.Forms.PrintDialog();
            this.printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
            this.symbolView1 = new Interface.SymbolView();
            this.symbolView2 = new Interface.SymbolView();
            this.compass1 = new Interface.Plot();
            this.menuStrip1.SuspendLayout();
            this.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(904, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.printToolStripMenuItem,
            this.printPreviewToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            this.loadFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadFileToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.loadFileToolStripMenuItem.Text = "&Open File...";
            this.loadFileToolStripMenuItem.Click += new System.EventHandler(this.loadFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(173, 6);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.printToolStripMenuItem.Text = "Print...";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.printToolStripMenuItem_Click);
            // 
            // printPreviewToolStripMenuItem
            // 
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.printPreviewToolStripMenuItem.Text = "Print Preview...";
            this.printPreviewToolStripMenuItem.Click += new System.EventHandler(this.printPreviewToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(173, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeLabelingToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // changeLabelingToolStripMenuItem
            // 
            this.changeLabelingToolStripMenuItem.Name = "changeLabelingToolStripMenuItem";
            this.changeLabelingToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.changeLabelingToolStripMenuItem.Text = "Change Labeling...";
            this.changeLabelingToolStripMenuItem.Click += new System.EventHandler(this.changeLabelingToolStripMenuItem_Click);
            // 
            // heckbertLabelsToolStripMenuItem
            // 
            this.heckbertLabelsToolStripMenuItem.Name = "heckbertLabelsToolStripMenuItem";
            this.heckbertLabelsToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // ContentPanel
            // 
            this.ContentPanel.BackColor = System.Drawing.Color.LightGray;
            this.ContentPanel.Controls.Add(this.splitContainer1);
            this.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentPanel.Location = new System.Drawing.Point(0, 24);
            this.ContentPanel.Name = "ContentPanel";
            this.ContentPanel.Padding = new System.Windows.Forms.Padding(3);
            this.ContentPanel.Size = new System.Drawing.Size(904, 619);
            this.ContentPanel.TabIndex = 1;
            this.ContentPanel.Resize += new System.EventHandler(this.ContentPanel_Resize);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.compass1);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(1, 2, 2, 2);
            this.splitContainer1.Size = new System.Drawing.Size(898, 613);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 10;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.symbolView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.symbolView2);
            this.splitContainer2.Size = new System.Drawing.Size(200, 613);
            this.splitContainer2.SplitterDistance = 305;
            this.splitContainer2.TabIndex = 0;
            // 
            // printDocument
            // 
            this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_PrintPage);
            // 
            // printDialog
            // 
            this.printDialog.AllowCurrentPage = true;
            this.printDialog.Document = this.printDocument;
            this.printDialog.UseEXDialog = true;
            // 
            // printPreviewDialog
            // 
            this.printPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog.Document = this.printDocument;
            this.printPreviewDialog.Enabled = true;
            this.printPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog.Icon")));
            this.printPreviewDialog.Name = "printPreviewDialog";
            this.printPreviewDialog.Visible = false;
            // 
            // symbolView1
            // 
            this.symbolView1.BackColor = System.Drawing.Color.Transparent;
            this.symbolView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.symbolView1.Location = new System.Drawing.Point(0, 0);
            this.symbolView1.Margin = new System.Windows.Forms.Padding(0);
            this.symbolView1.Name = "symbolView1";
            this.symbolView1.Padding = new System.Windows.Forms.Padding(2, 2, 1, 1);
            this.symbolView1.Size = new System.Drawing.Size(200, 305);
            this.symbolView1.State = null;
            this.symbolView1.TabIndex = 8;
            // 
            // symbolView2
            // 
            this.symbolView2.BackColor = System.Drawing.Color.Transparent;
            this.symbolView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.symbolView2.Location = new System.Drawing.Point(0, 0);
            this.symbolView2.Margin = new System.Windows.Forms.Padding(0);
            this.symbolView2.Name = "symbolView2";
            this.symbolView2.Padding = new System.Windows.Forms.Padding(2, 1, 1, 2);
            this.symbolView2.Size = new System.Drawing.Size(200, 304);
            this.symbolView2.State = null;
            this.symbolView2.TabIndex = 0;
            // 
            // compass1
            // 
            this.compass1.BackColor = System.Drawing.Color.White;
            this.compass1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compass1.DrawAxes = true;
            this.compass1.Location = new System.Drawing.Point(1, 2);
            this.compass1.Margin = new System.Windows.Forms.Padding(2);
            this.compass1.MinimumSize = new System.Drawing.Size(125, 100);
            this.compass1.Name = "compass1";
            this.compass1.Padding = new System.Windows.Forms.Padding(3);
            this.compass1.Size = new System.Drawing.Size(693, 609);
            this.compass1.State = null;
            this.compass1.TabIndex = 4;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 643);
            this.Controls.Add(this.ContentPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "Extended Labeling - Talbot et al.";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ContentPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel ContentPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heckbertLabelsToolStripMenuItem;
        private System.Drawing.Printing.PrintDocument printDocument;
        private System.Windows.Forms.PrintDialog printDialog;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private Interface.SymbolView symbolView1;
        private System.Windows.Forms.ToolStripMenuItem changeLabelingToolStripMenuItem;
        private Interface.SymbolView symbolView2;
        private Interface.Plot compass1;
    }
}

