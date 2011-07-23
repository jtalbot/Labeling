namespace Interface
{
    partial class ParamSliders
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.densitySlider = new System.Windows.Forms.TrackBar();
            this.densityLabel = new System.Windows.Forms.Label();
            this.fontSlider = new System.Windows.Forms.TrackBar();
            this.fontLabel = new System.Windows.Forms.Label();
            this.densityBox = new System.Windows.Forms.TextBox();
            this.fontBox = new System.Windows.Forms.TextBox();
            this.minor = new System.Windows.Forms.CheckBox();
            this.j = new System.Windows.Forms.CheckBox();
            this.endpoints = new System.Windows.Forms.CheckBox();
            this.wks = new System.Windows.Forms.CheckBox();
            this.extraSteps = new System.Windows.Forms.CheckBox();
            this.qtrs = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.densitySlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fontSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // densitySlider
            // 
            this.densitySlider.Location = new System.Drawing.Point(43, 3);
            this.densitySlider.Maximum = 200;
            this.densitySlider.Minimum = 10;
            this.densitySlider.Name = "densitySlider";
            this.densitySlider.Size = new System.Drawing.Size(104, 45);
            this.densitySlider.TabIndex = 1;
            this.densitySlider.TickFrequency = 10;
            this.densitySlider.Value = 100;
            this.densitySlider.Scroll += new System.EventHandler(this.densitySlider_Scroll);
            // 
            // densityLabel
            // 
            this.densityLabel.AutoSize = true;
            this.densityLabel.Location = new System.Drawing.Point(2, 14);
            this.densityLabel.Name = "densityLabel";
            this.densityLabel.Size = new System.Drawing.Size(40, 13);
            this.densityLabel.TabIndex = 2;
            this.densityLabel.Text = "density";
            // 
            // fontSlider
            // 
            this.fontSlider.Location = new System.Drawing.Point(43, 63);
            this.fontSlider.Maximum = 30;
            this.fontSlider.Minimum = 7;
            this.fontSlider.Name = "fontSlider";
            this.fontSlider.Size = new System.Drawing.Size(104, 45);
            this.fontSlider.TabIndex = 3;
            this.fontSlider.Value = 12;
            this.fontSlider.Scroll += new System.EventHandler(this.fontSlider_Scroll);
            // 
            // fontLabel
            // 
            this.fontLabel.AutoSize = true;
            this.fontLabel.Location = new System.Drawing.Point(2, 74);
            this.fontLabel.Name = "fontLabel";
            this.fontLabel.Size = new System.Drawing.Size(46, 13);
            this.fontLabel.TabIndex = 4;
            this.fontLabel.Text = "font size";
            // 
            // densityBox
            // 
            this.densityBox.Location = new System.Drawing.Point(154, 14);
            this.densityBox.Name = "densityBox";
            this.densityBox.Size = new System.Drawing.Size(43, 20);
            this.densityBox.TabIndex = 5;
            this.densityBox.Text = "100";
            this.densityBox.TextChanged += new System.EventHandler(this.densityBox_TextChanged);
            // 
            // fontBox
            // 
            this.fontBox.Location = new System.Drawing.Point(154, 66);
            this.fontBox.Name = "fontBox";
            this.fontBox.Size = new System.Drawing.Size(43, 20);
            this.fontBox.TabIndex = 6;
            this.fontBox.Text = "12";
            this.fontBox.TextChanged += new System.EventHandler(this.fontBox_TextChanged);
            // 
            // minor
            // 
            this.minor.AutoSize = true;
            this.minor.Checked = true;
            this.minor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.minor.Location = new System.Drawing.Point(5, 113);
            this.minor.Name = "minor";
            this.minor.Size = new System.Drawing.Size(51, 17);
            this.minor.TabIndex = 7;
            this.minor.Text = "minor";
            this.minor.UseVisualStyleBackColor = true;
            this.minor.CheckedChanged += new System.EventHandler(this.minor_CheckedChanged);
            // 
            // j
            // 
            this.j.AutoSize = true;
            this.j.Checked = true;
            this.j.CheckState = System.Windows.Forms.CheckState.Checked;
            this.j.Location = new System.Drawing.Point(79, 113);
            this.j.Name = "j";
            this.j.Size = new System.Drawing.Size(28, 17);
            this.j.TabIndex = 8;
            this.j.Text = "j";
            this.j.UseVisualStyleBackColor = true;
            this.j.CheckedChanged += new System.EventHandler(this.j_CheckedChanged);
            // 
            // endpoints
            // 
            this.endpoints.AutoSize = true;
            this.endpoints.Checked = true;
            this.endpoints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.endpoints.Location = new System.Drawing.Point(132, 113);
            this.endpoints.Name = "endpoints";
            this.endpoints.Size = new System.Drawing.Size(72, 17);
            this.endpoints.TabIndex = 9;
            this.endpoints.Text = "endpoints";
            this.endpoints.UseVisualStyleBackColor = true;
            this.endpoints.CheckedChanged += new System.EventHandler(this.endpoints_CheckedChanged);
            // 
            // wks
            // 
            this.wks.AutoSize = true;
            this.wks.Checked = true;
            this.wks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wks.Location = new System.Drawing.Point(5, 150);
            this.wks.Name = "wks";
            this.wks.Size = new System.Drawing.Size(57, 17);
            this.wks.TabIndex = 10;
            this.wks.Text = "weeks";
            this.wks.UseVisualStyleBackColor = true;
            this.wks.CheckedChanged += new System.EventHandler(this.wks_CheckedChanged);
            // 
            // extraSteps
            // 
            this.extraSteps.AutoSize = true;
            this.extraSteps.Location = new System.Drawing.Point(132, 150);
            this.extraSteps.Name = "extraSteps";
            this.extraSteps.Size = new System.Drawing.Size(76, 17);
            this.extraSteps.TabIndex = 11;
            this.extraSteps.Text = "extraSteps";
            this.extraSteps.UseVisualStyleBackColor = true;
            this.extraSteps.CheckedChanged += new System.EventHandler(this.extraSteps_CheckedChanged);
            // 
            // qtrs
            // 
            this.qtrs.AutoSize = true;
            this.qtrs.Checked = true;
            this.qtrs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.qtrs.Location = new System.Drawing.Point(79, 150);
            this.qtrs.Name = "qtrs";
            this.qtrs.Size = new System.Drawing.Size(43, 17);
            this.qtrs.TabIndex = 12;
            this.qtrs.Text = "qtrs";
            this.qtrs.UseVisualStyleBackColor = true;
            this.qtrs.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // ParamSliders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.qtrs);
            this.Controls.Add(this.extraSteps);
            this.Controls.Add(this.wks);
            this.Controls.Add(this.endpoints);
            this.Controls.Add(this.j);
            this.Controls.Add(this.minor);
            this.Controls.Add(this.fontBox);
            this.Controls.Add(this.densityBox);
            this.Controls.Add(this.fontLabel);
            this.Controls.Add(this.fontSlider);
            this.Controls.Add(this.densityLabel);
            this.Controls.Add(this.densitySlider);
            this.Name = "ParamSliders";
            this.Size = new System.Drawing.Size(211, 179);
            ((System.ComponentModel.ISupportInitialize)(this.densitySlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fontSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar densitySlider;
        private System.Windows.Forms.Label densityLabel;
        private System.Windows.Forms.TrackBar fontSlider;
        private System.Windows.Forms.Label fontLabel;
        private System.Windows.Forms.TextBox densityBox;
        private System.Windows.Forms.TextBox fontBox;
        private System.Windows.Forms.CheckBox minor;
        private System.Windows.Forms.CheckBox j;
        private System.Windows.Forms.CheckBox endpoints;
        private System.Windows.Forms.CheckBox wks;
        private System.Windows.Forms.CheckBox extraSteps;
        private System.Windows.Forms.CheckBox qtrs;

    }
}
