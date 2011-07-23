using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Language;
using Renderer;
using Data;
using System.Dynamic;

namespace Interface
{
    public partial class Panel : UserControl
    {
        public IEnumerable<Layer> Layers { get; set; }

        public Panel(IEnumerable<Layer> layers)
        {
            InitializeComponent();

            this.Layers = layers;
            
            foreach (Layer l in Layers)
            {
                l.OnInvalidate += OnLayerInvalidate;
            }
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint |
               ControlStyles.OptimizedDoubleBuffer | 
               ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }

        public void Measure()
        {
            foreach (Layer layer in Layers)
            {
                layer.Layout(this.ClientRectangle);
            }
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillRectangle(new SolidBrush(Color.White), this.ClientRectangle);

            if (Layers.Count() > 0)
                Layers.First().Draw(g, this.ClientRectangle);
            
            /*foreach (Layer layer in Layers)
            {
                try
                {
                    layer.Draw(e.Graphics, this.ClientRectangle);
                }
                catch(Exception exception)
                {
                }
            }*/

            if (dragRectangle != Rectangle.Empty)
            {
                Pen pen = new Pen(Color.LightBlue, 1);
                Brush brush = new SolidBrush(Color.FromArgb(32, Color.LightBlue));
                g.FillRectangle(brush, dragRectangle);
                g.DrawRectangle(pen, dragRectangle);
            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.Graphics);
        }

        private void Panel_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        // Drag stuff
        Rectangle dragLimit = Rectangle.Empty;
        Point dragStart;
        Rectangle dragRectangle;
        
        // Double click stuff
        Rectangle doubleClickRectangle;
        Timer doubleClickTimer = new Timer();
        EventHandler doubleClickHandler = null;

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (doubleClickHandler == null && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Size doubleClickSize = SystemInformation.DoubleClickSize;
                doubleClickRectangle = new Rectangle(new Point(e.X - doubleClickSize.Width / 2, e.Y - doubleClickSize.Height / 2), doubleClickSize);
                doubleClickTimer.Interval = SystemInformation.DoubleClickTime;
                doubleClickHandler = new EventHandler((o, args) => realMouseDown(sender, e, false));
                doubleClickTimer.Tick += doubleClickHandler;
                doubleClickTimer.Start();
            }
            else
            {
                if (doubleClickRectangle.Contains(e.Location))
                {
                    realMouseDown(sender, e, true);
                }
                else
                {
                    if(doubleClickHandler != null)
                        doubleClickHandler(null, null);
                    realMouseDown(sender, e, false);
                }
            }
        }
         
        private void realMouseDown(object sender, MouseEventArgs e, bool isDoubleClick)
        {
            if (!isDoubleClick)
            {                
                bool update = false;
                //foreach (Layer layer in Layers)
                if (Layers.Count() > 0)
                {
                    update = Layers.First().NotifyMouseDown(this.ClientRectangle, e, Control.ModifierKeys) || update;
                }
                if (e.Button == MouseButtons.Left)
                {
                    Size dragSize = SystemInformation.DragSize;
                    dragLimit = new Rectangle(new Point(e.X - dragSize.Width / 2, e.Y - dragSize.Height / 2), dragSize);
                    dragStart = new Point(e.X, e.Y);
                    update = true || update;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    dragLimit = Rectangle.Empty;

                    IEnumerable<ToolStripItem> items = Enumerable.Empty<ToolStripItem>();
                    foreach (Layer layer in Layers)
                    {
                        items = items.Union(layer.NotifyContextMenu(this.ClientRectangle, e));
                    }
                    if (items.Count() > 0)
                    {
                        ContextMenuStrip contextMenu = new ContextMenuStrip();
                        contextMenu.Items.AddRange(items.ToArray());
                        contextMenu.Show(this, e.X, e.Y);
                    }
                }
                if (update)
                    this.Refresh();
            }

            if (doubleClickHandler != null)
            {
                doubleClickTimer.Tick -= doubleClickHandler;
                doubleClickHandler = null;
                doubleClickRectangle = Rectangle.Empty;
            }
            doubleClickTimer.Stop();
        }

        private void Panel_MouseEnter(object sender, EventArgs e)
        {
            // do nothing for now...
        }

        private void Panel_MouseLeave(object sender, EventArgs e)
        {
            bool update = false;
            foreach (Layer layer in Layers)
            {
                update = layer.NotifyMouseLeave(this.ClientRectangle, e) || update;
            }
            if (update)
                this.Refresh();
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (doubleClickHandler != null && !doubleClickRectangle.Contains(e.Location))
                doubleClickHandler(null, null);

            bool update = false;

            if (e.Button == MouseButtons.Left)
            {
                if (dragLimit != Rectangle.Empty && !dragLimit.Contains(e.X, e.Y))
                {
                    // we are dragging!
                    dragRectangle = new Rectangle(Math.Min(dragStart.X, e.X), Math.Min(dragStart.Y, e.Y), Math.Abs(dragStart.X - e.X), Math.Abs(dragStart.Y - e.Y));
                }
                else
                {
                    dragRectangle = Rectangle.Empty;
                }
                update = true || update;

                //foreach (Layer layer in Layers)                    
            }

            if (Layers.Count() > 0)
            {
                if (dragRectangle == Rectangle.Empty)
                    update = Layers.First().NotifyMouseMove(this.ClientRectangle, e) || update;
                else
                    update = Layers.First().NotifyMouseSelect(this.ClientRectangle, e, Control.ModifierKeys, dragRectangle) || update;
            }

            if (update)
                this.Refresh();
        }

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            bool update = false;
            if (dragRectangle != Rectangle.Empty)
            {
                dragLimit = Rectangle.Empty;
                dragRectangle = Rectangle.Empty;
                update = true || update;
            }

            //foreach (Layer layer in Layers)
            if (Layers.Count() > 0)
            {
                update = Layers.First().NotifyMouseUp(this.ClientRectangle, e) || update;
            }
            if (update)
                this.Refresh();
        }

        private void OnLayerInvalidate(Layer sender)
        {
            this.Invalidate();
            //modeler.NotifyDisplayChanged();            
        }
        
    }
}
