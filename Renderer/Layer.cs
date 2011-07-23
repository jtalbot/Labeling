using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;
using System.Drawing;
using System.Windows.Forms;
using Data;

namespace Renderer
{
    public enum SelectionMode { REPLACE, ADD, REMOVE, TOGGLE };
    public delegate bool LayerInteraction(Point point, Rectangle rec, IEnumerable<Mark> hits);
    public delegate bool LayerSelection(IEnumerable<Mark> payloads, SelectionMode mode);
    public delegate bool LayerSelectionEnded();
    public delegate IEnumerable<ToolStripItem> LayerContextMenu(Point point, Rectangle rec, IEnumerable<Mark> hits);
    public delegate void InvalidateHandler(Layer layer);

    public abstract class Layer
    {
        protected PlotPanel plotPanel;
        protected RenderState state;

        protected LayerContextMenu OnContextMenu;
        public InvalidateHandler OnInvalidate;

        public Layer(PlotPanel plotPanel)
        {
            this.plotPanel = plotPanel;
            this.state = plotPanel.state;
        }

        public virtual void Layout(Rectangle screen)
        {
            // do nothing...override if you need to recompute layout on resize (currently only the axes need to do this)
        }
        
        public void Draw(Graphics g, Rectangle rec)
        {
            plotPanel.setScreen(rec);
            List<Tuple<Mark, Transform, RectangleF, double>> items = DrawVisitor(plotPanel, g.ClipBounds, 0);
            foreach (var mt in items.OrderBy(a => a.Item4))
            {
                g.Clip = new Region(mt.Item3);
                mt.Item1.render(g, mt.Item2);
            }
        }

        public List<Tuple<Mark, Transform, RectangleF, double>> DrawVisitor(Panel p, RectangleF clip, double order)
        {
            List<Tuple<Mark, Transform, RectangleF, double>> result = new List<Tuple<Mark, Transform, RectangleF, double>>();
            foreach (Mark m in p.Children)
            {
                if (m is Panel)
                {
                    RectangleF r = m.bounds(p.ToScreen());
                    r.Width += 1;
                    r.Height += 1;
                    r.Intersect(clip);

                    result.AddRange(DrawVisitor(m as Panel, (m as Panel).Clip ? r : clip, /*order + */m.Order));
                }
                result.Add(new Tuple<Mark, Transform, RectangleF, double>(m, p.ToScreen(), clip, /*order + */m.Order));
            }
            return result;
        }

        public void invalidate()
        {
            if (OnInvalidate != null)
                OnInvalidate(this);
        }

        public bool NotifyMouseMove(Rectangle rec, MouseEventArgs e)
        {
            // walk the tree and tell them what to do...
            plotPanel.setScreen(rec);
            bool exit = NotifyMouseExitVisitor(plotPanel, e);
            bool enter = NotifyMouseEnterVisitor(plotPanel, e);
            return exit || enter;
        }

        private bool NotifyMouseExitVisitor(Panel p, MouseEventArgs e)
        {
            Point test = new Point(e.X, e.Y);
                
            bool update = false;
            foreach(Mark m in p.Children)
            {
                if(m is Panel)
                {
                    update = NotifyMouseExitVisitor(m as Panel, e) || update;
                }
                
                if(m.hovered)
                {                        
                    if (!m.hitTest(test, p.ToScreen()))
                    {
                        update = m.MouseExit(p.ToScreen().untransform(test)) || update;
                        m.hovered = false;
                    }
                }
            }

            return update;
        }

        private bool NotifyMouseEnterVisitor(Panel p, MouseEventArgs e)
        {
            bool update = false;

            Point test = new Point(e.X, e.Y);
            foreach (Mark m in p.Children)
            {
                if (m is Panel)
                {
                    if(!(m as Panel).Clip || (m as Panel).bounds(p.ToScreen()).Contains(test))
                        update = NotifyMouseEnterVisitor(m as Panel, e) || update;
                }
                if (!m.hovered)
                {
                    if (m.hitTest(test, p.ToScreen()))
                    {
                        update = m.MouseEnter(p.ToScreen().untransform(test)) || update;
                        m.hovered = true;
                    }
                }
                if (m.hovered)
                {
                    update = m.MouseOver(p.ToScreen().untransform(test)) || update;
                }
            }
        
            return update;
        }

        public bool NotifyMouseLeave(Rectangle rec, EventArgs e)
        {
            plotPanel.setScreen(rec);
            return NotifyMouseLeaveVisitor(plotPanel, e);
        }

        public bool NotifyMouseLeaveVisitor(Panel p, EventArgs e)
        {
            bool update = false;
            foreach (Mark m in p.Children)
            {
                if (m is Panel)
                {
                    update = NotifyMouseLeaveVisitor(m as Panel, e) || update;
                }

                if (m.hovered)
                {
                    update = m.MouseExit(new PointF(float.NegativeInfinity, float.NegativeInfinity)) || update;

                    m.hovered = false;
                }
            }

            return update;
        }

        private SelectionMode SelectionModeFromKeys(Keys modifierKeys)
        {
            if (modifierKeys == Keys.Control)
                return SelectionMode.ADD;
            else if (modifierKeys == Keys.Alt)
                return SelectionMode.REMOVE;
            else
                return SelectionMode.REPLACE;
        }

        public bool NotifyMouseSelect(Rectangle rec, MouseEventArgs e, Keys modifierKeys, Rectangle selection)
        {
            plotPanel.setScreen(rec);
            var s = plotPanel.hit(selection, rec);

            SelectionMode mode = SelectionModeFromKeys(modifierKeys);

            bool update = false;

            foreach (var manager in plotPanel.selectionManagers)
            {
                update = manager.InteractiveUpdate(mode, s.Where(m=>m.SelectionManager == manager)) || update;
            }

            return update;
        }

        public bool NotifyMouseDown(Rectangle rec, MouseEventArgs e, Keys modifierKeys)
        {
            bool update = false;

            SelectionMode mode = SelectionModeFromKeys(modifierKeys);

            var s = plotPanel.hit(new Point(e.X, e.Y), rec);

            if (e.Button == MouseButtons.Left)
            {
                foreach (var manager in plotPanel.selectionManagers)
                {
                    update = manager.InteractiveUpdate(mode, s.Where(m=>m.SelectionManager == manager)) || update;
                }
            }

            return update;
        }

        public bool NotifyMouseUp(Rectangle rec, MouseEventArgs e)
        {
            bool update = false;

            foreach (var manager in plotPanel.selectionManagers)
            {
                update = manager.Done(true) || update;
            }

            return update;
        }

        public IEnumerable<ToolStripItem> NotifyContextMenu(Rectangle rec, MouseEventArgs e)
        {
            IEnumerable<ToolStripItem> items = Enumerable.Empty<ToolStripItem>();
            if (OnContextMenu != null)
            {
                Point point = new Point(e.X, e.Y);

                //IEnumerable<Mark> payloads = hitTest(point, rec);

                //if (payloads.Count() > 0)
                //{
                //    items = OnContextMenu(point, rec, payloads);
                //}                   
            }
            return items;
        }


        public static string FormatNum(double d, double range)
        {
            int digits = range >= 0 ?
               (int)Math.Floor(Math.Log10(range)) - 1 :
               (int)Math.Floor(Math.Log10(-range)) - 1;

            double scale = Math.Pow(10, digits);
            double num = Math.Round(d / scale) * scale;

            int decimals = digits < 0 ? -digits : 0;

            string format = string.Format("{{0:f{0}}}", decimals);
            return string.Format(format, num);
        }

        /*public static string FormatNum(double d, Value s)
        {
            double t = (s is TransformedVariable) ? (s as TransformedVariable).evaluateInverse(d) : d;
            return FormatNum(t, (s.Range.max - s.Range.min) / 10 / s.Derivative(d));
        }*/
    }
}
