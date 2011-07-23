using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using Layout;

namespace Main
{
    public partial class LabelSelection : Form
    {
        State state;
        public State State
        {
            get { return state; }
            set
            {
                state = value;                
            }
        }

        public LabelSelection()
        {
            InitializeComponent();

            radioButton1.Checked = (AxisLayout.algorithm == AxisLayout.Algorithm.OURS);
            radioButton2.Checked = (AxisLayout.algorithm == AxisLayout.Algorithm.WILKINSON);
            radioButton3.Checked = (AxisLayout.algorithm == AxisLayout.Algorithm.HECKBERT);
            radioButton5.Checked = (AxisLayout.algorithm == AxisLayout.Algorithm.MATPLOTLIB);

            hScrollBar1.Value = (int)(AxisLayout.AxisDensity*10000);
            hScrollBar2.Value = (int)AxisLayout.AxisFontSize;
        }

        RadioButton checkedRadioButton;

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                if (radioButton.Checked)
                {
                    checkedRadioButton = radioButton;
                }
                else if (checkedRadioButton == radioButton)
                {
                    checkedRadioButton = null;
                }

                if (checkedRadioButton == radioButton1)
                    AxisLayout.algorithm = AxisLayout.Algorithm.OURS;
                else if(checkedRadioButton == radioButton2)
                    AxisLayout.algorithm = AxisLayout.Algorithm.WILKINSON;
                else if (checkedRadioButton == radioButton3)
                    AxisLayout.algorithm = AxisLayout.Algorithm.HECKBERT;
                else if (checkedRadioButton == radioButton5)
                    AxisLayout.algorithm = AxisLayout.Algorithm.MATPLOTLIB;

                if (checkedRadioButton != null && state != null)
                    state.NotifyDisplayChanged();
            }
        }

        private void hScrollBar2_ValueChanged(object sender, EventArgs e)
        {
            AxisLayout.AxisFontSize = hScrollBar2.Value;
            if(state != null)
                state.NotifyDisplayChanged();
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            AxisLayout.AxisDensity = hScrollBar1.Value/10000.0;
            if (state != null)
                state.NotifyDisplayChanged();
        } 
    }
}
