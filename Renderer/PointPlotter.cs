using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Language;
using Plotter;
using System.Drawing;
using Data.Source;
using System.Windows.Forms;

namespace Plotter
{
    public class PointPlotter : Plotter
    {
        DataSet dataSet;

        IEnumerable<string> labeled = new List<string>();

        public PointPlotter(Modeler modeler, DataSet dataSet)
            : base(modeler)
        {
            this.dataSet = dataSet;
        }

        protected override object computePanelData(Term x, Term y, IList<Term> d, Factor.Level[] conditionalOn)
        {
            return dataSet.conditionOn(conditionalOn);
        }

        protected override List<PlotElement> plot(Coplot xplot, Coplot yplot, IList<Term> d, Factor.Level[] conditionalOn, object data, PlotOptions options)
        {
            Term x = xplot.dim, y = yplot.dim;

            List<PlotElement> result = new List<PlotElement>();

            //if (x is Factor || y is Factor)
              //  return result;

            List<double> xp, yp;
            DataSet ds = data as DataSet;

            var points = ds.evaluate(new Term[] { x, y });

            xp = (from a in points[0] select Convert.ToDouble(a)).ToList();
            yp = (from a in points[1] select Convert.ToDouble(a)).ToList();
            
            arrangeBins(x, y, xp, yp);

            var labels = ds.evaluate(d.ToArray());

            for (int i = 0; i < xp.Count(); i++)
            {
                List<double> x1 = new List<double>(), y1 = new List<double>();
                x1.Add(xp[i]);
                y1.Add(yp[i]);
                if (labeled.Contains(ds.RowNames[i]))
                {
                    List<string> ls = new List<string>();
                    for (int k = 0; k < d.Count(); k++)
                    {
                        if (d[k] is Factor)
                            ls.Add((d[k] as Factor)[Convert.ToInt32(labels[k][i])].ToString());
                        else
                            ls.Add(Convert.ToString(labels[k][i]));
                    }
                    string label = String.Join(", ", ls.ToArray());
                    result.Add(new PlotElement(x1, y1, PlotElement.PlotType.POINTS, label, Color.Goldenrod));
                    result.Last().payload = ds.RowNames[i];
                }
                else
                {
                    result.Add(new PlotElement(x1, y1, PlotElement.PlotType.POINTS, "", options.DrawEstimateInterval ? 75 : 75));
                    result.Last().payload = ds.RowNames[i];
                }
            }
           
            return result;
        }

        protected override bool OnMouseOver(IEnumerable<object> payloads)
        {
            labeled = from p in payloads select Convert.ToString(p);
            if (labeled.Count() > 0)
                return true;
            else
                return false;
        }

        protected override bool OnMouseExit()
        {
            labeled = new List<string>();
            return true;
        }

        void arrangeBins(Term x, Term y, List<double> xs, List<double> ys)
        {
            if (xs.Count() <= 0 || ys.Count() <= 0)
                return;

            if((x is Constant) && !(y is Constant))
            {
                IEnumerable<double> allys = from d in dataSet.evaluate(y) select Convert.ToDouble(d);
                var groups = from e in allys group e by e into gr select new { Value = gr.Key, Count = gr.Count() };
                int max = groups.Max(a => a.Count);

                //var d = (from e in ys group e by e into gr select new { Value = gr.Key, Count = gr.Count(), used = 0 }).ToDictionary(a => a.Value);
                Dictionary<double,int> offset = new Dictionary<double,int>();
                //int max = dataSet.NRows;
                for(int i = 0; i < ys.Count(); i++)
                {
                    if(!offset.ContainsKey(ys[i]))
                        offset.Add(ys[i], 0);
                    xs[i] += 0.5 * offset[ys[i]] / max;
                    offset[ys[i]] = offset[ys[i]] + 1;
                }
            }
            else if (!(x is Constant) && (y is Constant))
            {
                IEnumerable<double> allxs = from d in dataSet.evaluate(x) select Convert.ToDouble(d);
                var groups = from e in allxs group e by e into gr select new { Value = gr.Key, Count = gr.Count() };
                int max = groups.Max(a => a.Count);

                //var d = (from e in ys group e by e into gr select new { Value = gr.Key, Count = gr.Count(), used = 0 }).ToDictionary(a => a.Value);
                Dictionary<double, int> offset = new Dictionary<double, int>();
                //int max = dataSet.NRows;
                for (int i = 0; i < xs.Count(); i++)
                {
                    if (!offset.ContainsKey(xs[i]))
                        offset.Add(xs[i], 0);
                    ys[i] += 0.5 * offset[xs[i]] / max;
                    offset[xs[i]] = offset[xs[i]] + 1;
                }
            }
            else if ((x is Constant) && (y is Constant))
            {
                int max = dataSet.NRows;
                double size = Math.Sqrt((double)xs.Count() / max);
                int count = (int)Math.Ceiling(Math.Sqrt(xs.Count()));


                for (int i = 0; i < xs.Count(); i++)
                {
                    int dx = i / count;
                    int dy = i % count;

                    xs[i] = 0.5 + ((double)dx / count) * size - size / 2;
                    ys[i] = 0.5 + ((double)dy / count) * size - size / 2;
                }
            }
        }
    }
}
