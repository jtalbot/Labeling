using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Language;
using Plot;
using Layout;


namespace Interface
{
    public partial class ParamSliders : UserControl
    {
        Language.Model modeler;
        public Language.Model Modeler
        {
            get { return modeler; }
            set
            {
                modeler = value;
            }
        }

        public ParamSliders()
        {
            InitializeComponent();
        }

        private void densitySlider_Scroll(object sender, EventArgs e)
        {
            int density = densitySlider.Value;
            if (densityBox.ToString() != densityBox.Text)
                this.densityBox.Text = density.ToString();
        }

        private void fontSlider_Scroll(object sender, EventArgs e)
        {
            int fontSize = fontSlider.Value;
            if (fontSize.ToString() != fontBox.Text)
                this.fontBox.Text = fontSize.ToString();
            
        }

        private void fontBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int fontSize = Int32.Parse(fontBox.Text);
                if (fontSize >= fontSlider.Minimum && fontSize <= fontSlider.Maximum)
                {
                    fontSlider.Value = fontSize;
                    FrameLayer.IdealFontSize = fontSize;
                    modeler.NotifyPlotChanged();
                    
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void densityBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int density = Int32.Parse(densityBox.Text);
                if (density >= densitySlider.Minimum && density <= densitySlider.Maximum)
                {
                    densitySlider.Value = density;
                    FrameLayer.IdealLabelSpacing = density;
                    //modeler.NotifyDisplayChanged();
                    modeler.NotifyPlotChanged();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void minor_CheckedChanged(object sender, EventArgs e)
        {
            AxisLayout.ignoreMinorTicks = !minor.Checked;
        }

        private void j_CheckedChanged(object sender, EventArgs e)
        {
            AxisLayout.ignoreJ = !j.Checked;
        }

        private void endpoints_CheckedChanged(object sender, EventArgs e)
        {
            AxisLayout.labelEndpoints = endpoints.Checked;
        }

        private void wks_CheckedChanged(object sender, EventArgs e)
        {
            AxisLayout.useWeeks = wks.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            AxisLayout.useQuarters = qtrs.Checked;
        }

        private void extraSteps_CheckedChanged(object sender, EventArgs e)
        {
            AxisLayout.useExtraSteps = extraSteps.Checked;
        }
    }
}
