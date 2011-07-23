using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Language;
using System.Drawing;
using System.Windows.Forms;

namespace Renderer
{
    public class PointLayer : Layer
    {
        Panel panel;
        //PointSelectionManager psm;

        public PointLayer(PlotPanel plotPanel)
            : base(plotPanel)
        {
            //psm = new PointSelectionManager(dataSet);
            //plotPanel.selectionManagers.Add(psm);

            panel = new Panel().SetName("point panel");
            plotPanel.Plot.Add(panel);
            
            DoLayout();
        }

        public void DoLayout()
        {
            Vector x, y, color;
            x = state.Frame.P.Columns[0];
            y = state.Frame.R.Columns[0];
            color = state.Frame.P.Columns.Count() > 1 ? state.Frame.P.Columns[1] : new Constant(0, x.Length);
            /*if (coplot.Labels.Count() > 0)
            {
                var labels = this.ds.Select<string>(coplot.Labels);
                lp = labels[0];
            }*/

            List<Mark> points = new List<Mark>();
            List<Mark> labels = new List<Mark>();

            Color blue = Color.FromArgb(230, 90, 162, 255);
            List<string> lp = null;
                
            if (x is Numeric || y is Numeric)
            {
                List<double> xp = x.Select<double>();
                List<double> yp = y.Select<double>();

                for (int i = 0; i < xp.Count(); i++)
                {
                    LabelMark label = new LabelMark(
                        lp == null ? "" : lp[i], xp[i], yp[i], 
                        Color.Black, ContentAlignment.BottomLeft)
                        .SetVisible(false)
                        .SetOrder(500);
                    labels.Add(label);

                    Pen pen = new Pen(blue, 2);
                    points.Add(new DotMark(xp[i], yp[i])
                        //.SetSelectionManager(psm)
                        .SetStroke(pen)
                        .SetData(i)
                        .SetSize(7)
                        .SetShape(DotMark.Shape.CIRCLE)
                        //.SetFillColor(c)
                        .SetMouseEnter(p => { label.Visible = true; return true; })
                        .SetMouseExit(p => { label.Visible = false; return true; }));
                }
            }
            else
            {
                List<int> xp = x.Select<int>();
                List<int> yp = y.Select<int>();
                
                // bin to stacks...
                var zipped = xp.Zip(yp, (a, b) => new { x = a, y = b });
                var bins = zipped.GroupBy(t => t).Select(g => new { loc = g.Key, cnt = g.Count() });
                var max = bins.Max(a => a.cnt);

                bool vertical = xp.Max() >= yp.Max();

                foreach (var bin in bins)
                {
                    if (vertical)
                    {
                        points.Add(new BarMark(bin.loc.x - 0.4, bin.loc.y - 0.5, bin.loc.x + 0.4, bin.loc.y - 0.5 + (double)bin.cnt / max * 0.95)
                            .SetFillColor(blue));
                        labels.Add(new LabelMark("" + bin.cnt, bin.loc.x, bin.loc.y - 0.5 + (double)bin.cnt / max * 0.95, Color.Black, ContentAlignment.TopCenter));
                    }
                    else
                    {
                        points.Add(new BarMark(bin.loc.x - 0.5, bin.loc.y - 0.4, bin.loc.x - 0.5 + (double)bin.cnt / max * 0.95, bin.loc.y + 0.4)
                            .SetFillColor(blue));
                        labels.Add(new LabelMark("" + bin.cnt, bin.loc.x - 0.5 + (double)bin.cnt / max * 0.95, bin.loc.y, Color.Black, ContentAlignment.MiddleRight));
                    }
                }
            }                
            
            panel.Children = points.Concat(labels).ToList();
        }

        /*protected class PointSelectionManager : SelectionManager
        {
            DataSet dataSet;            
            
            public List<bool> selected = new List<bool>();

            public PointSelectionManager(DataSet dataSet)
            {
                this.dataSet = dataSet;

                if (!dataSet.hasColumn("imhotep.selection"))
                    dataSet.setColumn("imhotep.selection", "FALSE");

                if (!dataSet.hasColumn("imhotep.prevSelection"))
                    dataSet.setColumn("imhotep.prevSelection", "FALSE");

                selected = dataSet.Select<bool>("imhotep.selection");
            }

            public override bool InteractiveUpdate(SelectionMode mode, IEnumerable<Mark> marks)
            {
                List<string> points = marks.Select(m => m.Name).ToList();

                if (mode == SelectionMode.REPLACE)
                    dataSet.setColumn("imhotep.selection", "rownames(" + dataSet + ") %in% c(" + String.Join(",", points.Select(p => "\"" + p + "\"").ToArray()) + ")");
                else if (mode == SelectionMode.ADD)
                    dataSet.setColumn("imhotep.selection", "imhotep.prevSelection | (rownames(" + dataSet + ") %in% c(" + String.Join(",", points.Select(p => "\"" + p + "\"").ToArray()) + "))");
                else if (mode == SelectionMode.REMOVE)
                    dataSet.setColumn("imhotep.selection", "imhotep.prevSelection & !(rownames(" + dataSet + ") %in% c(" + String.Join(",", points.Select(p => "\"" + p + "\"").ToArray()) + "))");
                else if (mode == SelectionMode.TOGGLE)
                    dataSet.setColumn("imhotep.selection", "xor(imhotep.prevSelection, (rownames(" + dataSet + ") %in% c(" + String.Join(",", points.Select(p => "\"" + p + "\"").ToArray()) + ")))");

                selected = dataSet.Select<bool>("imhotep.selection");

                //we'll get notified by the dataSet changed event.
                return true;
            }

            public override bool Done(bool commit)
            {
                if (commit)
                {
                    if (dataSet.hasColumn("imhotep.selection"))
                        dataSet.setColumn("imhotep.prevSelection", "imhotep.selection");
                }
                else
                {
                    if (dataSet.hasColumn("imhotep.prevSelection"))
                        dataSet.setColumn("imhotep.selection", "imhotep.prevSelection");
                }
                return false;
            }
        }*/
    }
}
