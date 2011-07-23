namespace Interface
{
    partial class SymbolView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.list = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.list.IntegralHeight = false;
            this.list.ItemHeight = 18;
            this.list.Location = new System.Drawing.Point(0, 0);
            this.list.Margin = new System.Windows.Forms.Padding(0);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(200, 846);
            this.list.TabIndex = 0;
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            // 
            // SymbolView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.list);
            this.DoubleBuffered = true;
            this.Name = "SymbolView";
            this.Size = new System.Drawing.Size(200, 846);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox list;


    }
}
