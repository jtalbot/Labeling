using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Interface
{
    public partial class HeaderPanel : System.Windows.Forms.Panel
    {
        private HeaderLayout layoutEngine;
        public override LayoutEngine LayoutEngine
        {
            get
            {
                if (layoutEngine == null)
                {
                    layoutEngine = new HeaderLayout();
                }

                return layoutEngine;
            }
        }

        public HeaderPanel()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.UserPaint | 
              ControlStyles.ResizeRedraw, true);
        }
    }

    public class HeaderLayout : LayoutEngine
    {
        public override bool Layout(
            object container,
            LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;

            // Use DisplayRectangle so that parent.Padding is honored.
            Rectangle parentDisplayRectangle = parent.DisplayRectangle;
            Point nextControlLocation = parentDisplayRectangle.Location;

            foreach (Control c in parent.Controls)
            {
                // Only apply layout to visible controls.
                if (!c.Visible)
                {
                    continue;
                }

                // Respect the margin of the control:
                // shift over the left and the top.
                nextControlLocation.Offset(c.Margin.Left, c.Margin.Top);

                // Set the location of the control.
                c.Location = nextControlLocation;

                // Set the autosized controls to their 
                // autosized heights.
                if (c.AutoSize)
                {
                    c.Size = new Size(parentDisplayRectangle.Width, c.GetPreferredSize(parentDisplayRectangle.Size).Height);
                }

                if (!c.AutoSize && c == parent.Controls[parent.Controls.Count - 1])
                {
                    c.Size = new Size(parentDisplayRectangle.Width - c.Margin.Left - c.Margin.Right, parentDisplayRectangle.Bottom - nextControlLocation.Y - c.Margin.Bottom);
                }

                // Move X back to the display rectangle origin.
                nextControlLocation.X = parentDisplayRectangle.X;

                // Increment Y by the height of the control 
                // and the bottom margin.
                nextControlLocation.Y += c.Height + c.Margin.Bottom;
            }

            // Optional: Return whether or not the container's 
            // parent should perform layout as a result of this 
            // layout. Some layout engines return the value of 
            // the container's AutoSize property.
            return false;
        }
    }
}
