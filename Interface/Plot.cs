using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Data;
using Renderer;
using Language;
using System.Windows.Forms.Layout;
using System.Drawing.Drawing2D;

namespace Interface
{
    public partial class Plot : UserControl
    {
        State state;
        public State State
        {
            get { return state; }
            set {
                state = value;
                if (state != null)
                {
                    state.DataSetChanged += new DataSetChangedHandler(modeler_DataSetChanged);
                    state.DisplayChanged += new DisplayChangedHandler(modeler_DisplayChanged);
                }
            }
        }

        bool drawAxes = true;
        public bool DrawAxes
        {
            get { return drawAxes; }
            set
            {
                drawAxes = value;
                modeler_DisplayChanged();
            }
        }

        public Plot()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }

        void modeler_DataSetChanged(Frame dataSet)
        {
            modeler_DisplayChanged();
        }
        
        void modeler_DisplayChanged()
        {
            if (state != null)
            {
                generatePlot();
                this.Refresh();
            }
        }

        public void Draw(Graphics g, Rectangle bounds)
        {
            foreach (Control c in this.Controls)
            {
                if (c is Panel)
                {
                    (c as Panel).Draw(g);
                }
            }
        }

        void generatePlot()
        {
            //this.BeginUpdate();
            this.SuspendLayout();
            this.Controls.Clear();

            Renderer.RenderState renderState = new Renderer.RenderState();
            Formula f = new Formula(state.X, state.Y);
            renderState.Frame = f.Eval(state.DataSet).First() as ModelFrame;
            PlotPanel plotPanel = new PlotPanel(renderState);

            renderState.MarginLeft = 80;
            renderState.MarginBottom = 60;
            renderState.MarginRight = 20;
            renderState.MarginTop = 20;

            List<Layer> layers = new List<Layer>();
            layers.Add(new FrameLayer(plotPanel));
            layers.Add(new PointLayer(plotPanel));
            
            Panel p = new Panel(layers);
            p.Size = this.Size;
            p.Location = new Point(0, 0);
            p.Measure();

            this.Controls.Add(p);
            
            this.ResumeLayout();
            //this.EndUpdate();
        }

        
        public void OnKeyDown(Keys keyCode)
        {
            if (keyCode == Keys.P)
            {
                // take picture of current screen...
                //draw and save the image
                Bitmap screenshot = new Bitmap(this.Width, this.Height);
                Graphics g = Graphics.FromImage(screenshot);
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, this.Width, this.Height);

                DateTime dt = DateTime.Now;

                this.DrawToBitmap(screenshot, new Rectangle(0, 0, this.Width, this.Height));
                screenshot.Save("Screenshots/oneoff/image" + dt.Ticks + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public bool AllowHitTest()
        {
            return true;
        }

        private void Plot_Resize(object sender, EventArgs e)
        {
            modeler_DisplayChanged();
        }
    }
}
