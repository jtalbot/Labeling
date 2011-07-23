using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;
using System.Drawing;
using Data;
using Data.Source;
using System.Windows.Forms;

namespace Plot
{
    public class LoessLayer : Layer
    {
        DataSet dataSet;
        Language.Environment coplot;
        List<Mark> Fits = new List<Mark>(), Labels = new List<Mark>();
        Model model;
        Model.Slice slice;

        Panel panel;
        
        public LoessLayer(DataSet dataSet, Language.Environment coplot, PlotPanel plotPanel, PlotOptions options)
            : base(plotPanel, options)
        {
            this.dataSet = ((DataSet)coplot.TrainingData).where(coplot.Levels.ToArray());
            this.coplot = coplot;

            model = new Model(this.dataSet, coplot.ModelResponse, coplot.modelPredictors, coplot.modelType);
            slice = model.dataSlice(coplot.XSymbol());

            panel = new Panel().SetName("loess panel");
            plotPanel.Plot.Add(panel);

            DoLayout();
        }

        int lastSize = -1;
        public override void Layout(Rectangle screen)
        {
            // hide loess labels when we get small enough...(move to action?)
            int size = Math.Min(screen.Width, screen.Height);
            if(lastSize == -1 ||
                (size > 200) != (lastSize > 200))
            {
                if (size > 200 && Labels != null)
                    panel.Children = Fits.Union(Labels).ToList();
                else
                    panel.Children = Fits;
            }
            lastSize = size;
        }

        public void DoLayout()
        {
            Symbol x = coplot.XSymbol(), y = coplot.YSymbol();

            List<Mark> result = new List<Mark>();
            
            List<double> xp, yp;
            DataSet ds = dataSet.where(coplot.Levels.ToArray());

            var points = ds.Select<double>(x, y);

            xp = points[0];
            yp = points[1];
            
            if (xp.Count() == 0)
                return;

            if (options.DrawEstimateInterval || options.DrawPredictionInterval)
                return;

            // If both are variables do a loess fit
            if (x is Variable && y is Variable)
            {
                // Write X to new datasource so that it can be used in the loess formula as is (without transformations).                    
                DataSource newX = ds.Evaluate(x.Formula);
                List<List<double>> pxy;

                // loess only exists within the data range, so find new x min and max from the data range, not from the plot range
                double min = xp.Min() + (xp.Max() - xp.Min()) * 0.001, max = xp.Max() - (xp.Max() - xp.Min()) * 0.001;

                if(xp.Distinct().Count() < 4)
                {
                    // if we've got too few data points, just plot mean
                    pxy = new List<List<double>>();
                    pxy.Add(new List<double> {min, max});
                    pxy.Add(new List<double> {yp.Average(), yp.Average()});
                }
                if (xp.Distinct().Count() < 7 || x == y)
                {
                    // if we've got a few more, do a linear fit
                    DataSource lmModel = ds.Evaluate("lm(" + y.Formula + "~" + newX + ")");
                    pxy = lmModel.Select<double>(
                        "seq(" + min + "," + max + ",length=50)",           // X values
                        "predict(" + lmModel + ", newdata=data.frame(" + newX + "=seq(" + min + "," + max + ", length=50)))"); // Y values
                }
                else
                {
                    // if we have enough data points, do a loess fit
                    DataSource loessModel = ds.Evaluate("loess(" + y.Formula + "~" + newX + ", degree=1)");
                    pxy = loessModel.Select<double>(
                        "seq(" + min + "," + max + ",length=50)",           // X values
                        "predict(" + loessModel + ", newdata=data.frame(" + newX + "=seq(" + min + "," + max + ", length=50)))"); // Y values
                }

                List<double> xs = pxy[0];
                List<double> ys = pxy[1];

                //double rse = Math.Exp(-Convert.ToDouble(RSource.Instance.evaluate("loess.model", new string[] {"s"})[0][0]));
                double rse = 1;

                //if(options.DrawEstimate)
                    //result.Add(new LineMark(xs, ys).SetStrokeColor(Color.FromArgb(64, Color.FromArgb(148, 103, 189))));
                //else
                //    result.Add(new LineMark(xs, ys).SetStrokeColor(Color.FromArgb(255, 130, 115, 95))/*Color.FromArgb((int)(255 * rse), Color.FromArgb(128, Color.FromArgb(148, 103, 189))))*/);

                Fits = result;
                Labels = new List<Mark>();
            }
            // otherwise, do a density representation
            else
            {
                var fitandlabels = computeDensities(ds, x, y, xp, yp, 0.48);
                Fits = fitandlabels.Item1;
                Labels = fitandlabels.Item2;
            }

            panel.Children = Fits.Union(Labels).ToList();
        }

              /*          {
                var avgs = xp.Zip(yp, (a,b)=>new {first=a,second=b}).GroupBy(a=>a.first);
                foreach (var avg in avgs)
                {
                    double average = avg.Average(g=>g.second);
                    result.Add(new LineElement(avg.Key-0.5, avg.Key+0.5, average, average, Color.FromArgb(128, Color.LightGray)));
                }
            }*/

        Tuple<List<Mark>, List<Mark>> computeDensities(DataSet dataSet, Symbol x, Symbol y, List<double> xp, List<double> yp, double overallScale)
        {
            List<Mark> result = new List<Mark>();
            List<Mark> labels = new List<Mark>();

            /*if (x is Constant && y is Constant)
            {
                labels.Add(new LabelMark(" " + xp.Count(), 0, 0, Color.Gray, ContentAlignment.MiddleLeft));
            }*/
            if (x is Constant && y is Variable)
            {
                List<List<double>> density = dataSet.Evaluate("density(" + y.Formula + ")").Select<double>("x", "y");
                density[1] = density[1].Select(d => d / density[1].Max() * overallScale).ToList();
                result.Add(new AreaMark(density[1], density[0]).SetStrokeColor(Color.FromArgb(64, Color.LightGray)));
                result.Add(new LineMark(0, overallScale, yp.Average(), yp.Average()).SetStrokeColor(Color.White));
            }
            else if (x is Variable && y is Constant)
            {
                List<List<double>> density = dataSet.Evaluate("density(" + x.Formula + ")").Select<double>("x", "y");
                density[1] = density[1].Select(d => d / density[1].Max() * overallScale).ToList();
                result.Add(new AreaMark(density[0], density[1]).SetStrokeColor(Color.FromArgb(64, Color.LightGray)));
                result.Add(new LineMark(xp.Average(), xp.Average(), 0, overallScale).SetStrokeColor(Color.White));
            }
            /*else if (x is Factor && y is Constant)
            {
                List<double> counts = dataSet.Select<double>("table(" + (x as Factor).Formula + ")");
                double max = counts.Max();
                for (int i = 0; i < (x as Factor).Levels; i++)
                {
                    if (counts[i] > 0)
                    {
                        Mark plotElement = new LineMark(i, i, 0, (double)counts[i] / max * overallScale)
                            .SetStrokeWidth(8)
                            .SetStrokeColor(Color.FromArgb(128, Color.LightGray));
                        result.Add(plotElement);
                        labels.Add(new LabelMark(counts[i] + "", i, (double)counts[i] / max * overallScale, Color.Gray, ContentAlignment.BottomCenter));
                    }
                }
            }
            else if (y is Factor && x is Constant)
            {
                List<double> counts = dataSet.Select<double>("table(" + (y as Factor).Formula + ")");
                double max = counts.Max();
                for (int i = 0; i < (y as Factor).Levels; i++)
                {
                    if (counts[i] > 0)
                    {
                        Mark plotElement = new LineMark(0, (double)counts[i] / max * overallScale, i, i
                           ).SetStrokeWidth(8)
                           .SetStrokeColor(Color.FromArgb(128, Color.LightGray));
                        result.Add(plotElement);
                        labels.Add(new LabelMark(counts[i] + "", (double)counts[i] / max * overallScale, i, Color.Gray, ContentAlignment.MiddleLeft));
                    }
                }
            }*/
            else if (x is Factor && y is Variable)
            {
                List<double> counts = dataSet.Select<double>("table(" + (x as Factor).Formula + ")");
                DataSource densities = dataSet.By((x as Factor).Formula, "if(nrow(imhotep.arg) > 1) { density(" + y.Formula + ") } else {data.frame(x=numeric(0), y=numeric(0))}");
                double max = densities.SelectOne<double>("max(sapply(" + densities + ", function(imhotep.arg) { max(imhotep.arg$y) }))");

                var avgs = xp.Zip(yp, (a, b) => new { first = a, second = b }).GroupBy(a => a.first).Select(g => new { key=g.Key, avg=g.Select(c=>c.second).Average() });

                for (int i = 0; i < (x as Factor).Levels; i++)
                {
                    if (counts[i] > 1)
                    {
                        List<List<double>> density = densities.Select<double>(densities + "[[" + (i + 1) + "]]$x", densities + "[[" + (i + 1) + "]]$y");
                        density[1] = density[1].Select(d => d * 1/*((double)counts[i] / dataSet.Count)*/ / max * overallScale + i).ToList();
                        Mark plotElement = new AreaMark(density[1], density[0]).SetStrokeColor(Color.FromArgb(64, Color.LightGray));
                        result.Add(plotElement);
                        result.Add(new LineMark(i, i + overallScale, avgs.First(t => t.key == i).avg, avgs.First(t => t.key == i).avg).SetStrokeColor(Color.White));
                    }
                }
            }

            else if (y is Factor && x is Variable)
            {
                List<double> counts = dataSet.Select<double>("table(" + (y as Factor).Formula + ")");
                DataSource densities = dataSet.By((y as Factor).Formula, "if(nrow(imhotep.arg) > 1) { density(" + x.Formula + ") } else {data.frame(x=numeric(0), y=numeric(0))}");
                double max = densities.SelectOne<double>("max(sapply(" + densities + ", function(imhotep.arg) { max(imhotep.arg$y) }))");

                var avgs = yp.Zip(xp, (a, b) => new { first = a, second = b }).GroupBy(a => a.first).Select(g => new { key = g.Key, avg = g.Select(c => c.second).Average() });

                for (int i = 0; i < (y as Factor).Levels; i++)
                {
                    if (counts[i] > 1)
                    {
                        List<List<double>> density = densities.Select<double>(densities + "[[" + (i + 1) + "]]$x", densities + "[[" + (i + 1) + "]]$y");
                        density[1] = density[1].Select(d => d * 1/*((double)counts[i] / dataSet.Count)*/ / max * overallScale + i).ToList();
                        Mark plotElement = new AreaMark(density[0], density[1]).SetStrokeColor(Color.FromArgb(64, Color.LightGray));
                        result.Add(plotElement);
                        result.Add(new LineMark(avgs.First(t => t.key == i).avg, avgs.First(t => t.key == i).avg, i, i + overallScale).SetStrokeColor(Color.White));
                    }
                }
            }

            /*else if (x is Factor && y is Factor)
            {
                double max = dataSet.SelectOne<double>("max(table(" + (y as Factor).Formula + "," + (x as Factor).Formula + "))");

                foreach (Factor.Level ylevel in (y as Factor).AllLevels)
                {
                    DataSet conditioned = dataSet.where(ylevel);
                    List<double> counts = conditioned.Select<double>("table(" + (x as Factor).Formula + ")");
                    for (int i = 0; i < (x as Factor).Levels; i++)
                    {
                        if (counts[i] > 0)
                        {
                            Mark plotElement = new LineMark(i, i, ylevel.LevelIndex, ylevel.LevelIndex + (double)counts[i] / max * overallScale
                                ).SetStrokeWidth(8)
                                .SetStrokeColor(Color.FromArgb(128, Color.LightGray));
                            result.Add(plotElement);
                            if(counts[i] > 1)
                                labels.Add(new LabelMark(counts[i] + "", i, ylevel.LevelIndex + (double)counts[i] / max * overallScale, Color.Gray, ContentAlignment.BottomCenter));
                        }
                    }
                }
            }*/

            return new Tuple<List<Mark>, List<Mark>>(result, labels);
        }
    }
}
