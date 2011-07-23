using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Data;
using Language;
using Interface;
using System.IO;
using Renderer;
using Layout;
using System.Drawing.Printing;

namespace Main
{
    public partial class Main : Form
    {
        OpenFileDialog ofd;
        public State state = new State();
        LabelSelection ls = null;

        string workingDirectory = Directory.GetCurrentDirectory();

        public Main()
        {
            InitializeComponent();

            ofd = new OpenFileDialog();
            
            this.compass1.State = state;
            this.symbolView1.State = state;
            this.symbolView2.State = state;
            
            this.symbolView1.SymbolViewChanged += new SymbolViewChangedHandler(symbolView1_SymbolViewChanged);
            this.symbolView2.SymbolViewChanged += new SymbolViewChangedHandler(symbolView2_SymbolViewChanged);
            this.ofd.RestoreDirectory = true;
            this.ofd.InitialDirectory = workingDirectory + "\\DataSets";

            this.compass1.Focus();
        }

        void symbolView2_SymbolViewChanged()
        {
            state.X = symbolView2.GetSelected();
            state.NotifyDisplayChanged();
        }

        void symbolView1_SymbolViewChanged()
        {
            state.Y = symbolView1.GetSelected();
            state.NotifyDisplayChanged();
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ofd.ShowDialog() == DialogResult.OK)
            {
                Frame frame = Parser.CSV(this.ofd.FileName);
                frame = Frame.Cbind(new Frame(new Constant(0.0, frame.NumRows), Symbol.Constant), frame);
                state.DataSet = frame;
                state.SymbolTable = frame.Symbols;
                
                this.Text = "Imhotep - " + this.ofd.FileName;
            }
            this.compass1.Focus();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ContentPanel_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.printDialog.ShowDialog() == DialogResult.OK)
            {
                printDialog.Document.Print();
            } 
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.printPreviewDialog.ShowDialog();
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            PageSettings ps = e.PageSettings;            
            this.compass1.Draw(g, ps.Bounds);
        }

        private void changeLabelingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ls == null || ls.IsDisposed)
                ls = new LabelSelection();
            ls.State = state;
            ls.Visible = true;
        }
    }
}
