using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Language;
using System.Drawing;
using Data.Source;
using System.Windows.Forms;

namespace Plot
{
    public class ModelLayer : Layer
    {
        DataSet dataSet;
        Language.Environment coplot;
        Model model;
        List<Model.Slice> slices = new List<Model.Slice>();
        Model.Slice slice;
        List<Color> colors = new List<Color>();
        List<string> labels = new List<string>();

        public enum DisplayType { SLICED, MARGINAL_E, MARGINAL_PREDICTED, MARGINAL_RESIDUALS };
        private DisplayType displayType;

        Transform axisTransform;

        bool ShowXEstimate = false, ShowYEstimate = false;
        double HoveredX = 0, HoveredY = 0;

        Panel panel;
        
        public ModelLayer(DataSet dataSet, Language.Environment coplot, PlotPanel plotPanel, DisplayType displayType, PlotOptions options)
            : base(plotPanel, options)
        {
            this.dataSet = (DataSet) coplot.TrainingData;
            this.coplot = coplot;
            this.displayType = displayType;

            panel = new Panel().SetName("model panel");
            plotPanel.Plot.Add(panel);

            model = new Model(this.dataSet, coplot.ModelResponse, coplot.modelPredictors, coplot.modelType);
            if (coplot.ColorNest() is Constant)
            {
                slices.Add(model.slice(coplot.XSymbol(), coplot.Levels.ToArray()));
                colors.Add(Color.Orange);
                //colors.Add(Colors.encodeNestAsColor(coplot.ColorNest(), 0, true));
                labels.Add("");
            }
            else
            {
                Factor f = (coplot.ColorNest() is Factor) ? (coplot.ColorNest() as Factor) : (coplot.ColorNest() as Variable).partition(10);
                foreach (Factor.Level level in f.AllLevels)
                {
                    slices.Add(model.slice(coplot.XSymbol(), coplot.Levels.Union(new List<Factor.Level> { level }).ToArray()));
                    if (f.AllLevels.Count() == 1)
                        colors.Add(Color.Orange);
                    else
                        colors.Add(Colors.encodeNestAsColor(f, level.LevelIndex, false));
                    //colors.Add(Color.SandyBrown);
                    labels.Add(coplot.ColorNest().Name + ":" + level.ToString());
                }
            }

            slice = model.dataSlice(coplot.XSymbol());
            
            this.OnContextMenu += OnContextMenuHandler;

            DoLayout();
        }

        public void DoLayout()
        {
            List<Mark> pes = new List<Mark>();

            if (displayType == DisplayType.SLICED)
            {
                int i = 0;
                foreach (Model.Slice slice in slices)
                {
                    pes.AddRange(draw(slice, colors[i], labels[i]));
                    i++;
                }
            }
            else if (displayType == DisplayType.MARGINAL_E)
            {
                if (options.DrawEstimate)
                {
                    if (coplot.XSymbol() is Variable)
                    {
                        Symbol x = coplot.XSymbol(), y = coplot.YSymbol();
                        DataSet ds = dataSet.where(coplot.Levels.ToArray());
                        DataSource newX = ds.Evaluate(x.Formula);

                        var points = ds.Select<double>(x, y);

                        List<double> xp, yp;
                        xp = points[0];
                        yp = points[1];

                        double min = xp.Min() + (xp.Max() - xp.Min()) * 0.001, max = xp.Max() - (xp.Max() - xp.Min()) * 0.001;

                        for (int k = 0; k < 50; k++)
                        {
                            DataSource mm = ds.Evaluate("loess(" + slice.Formula + "$qi$ev[" + k + ",]" + "~" + newX + ", degree=1)");
                            var m = mm.Select<double>(
                                "seq(" + min + "," + max + ",length=50)",           // X values
                                "predict(" + mm + ", newdata=data.frame(" + newX + "=seq(" + min + "," + max + ", length=50)))");

                            pes.Add(new LineMark(m[0], m[1]).SetStrokeColor(Color.FromArgb(40, Color.Orange)));
                        }
                    }
                }
            }
            else if (displayType == DisplayType.MARGINAL_RESIDUALS)
            {
                if (options.DrawEstimate)
                {
                    Symbol x = coplot.XSymbol(), y = coplot.YSymbol();
                    DataSet ds = dataSet.where(coplot.Levels.ToArray());
                    DataSource newX = ds.Evaluate(x.Formula);

                    List<double>  mp = model.yValues(slice);
                    if (mp.Count() > 0)
                    {
                        mp = ds.RowNames.Select(s => (coplot.TrainingData as DataSet).RowNames.Contains(s) ?
                            mp[(coplot.TrainingData as DataSet).RowNames.IndexOf(s)] : double.PositiveInfinity).ToList();
                    }

                    var points = ds.Select<double>(x, y);

                    List<double> xp, yp;
                    xp = points[0];
                    yp = points[1];

                    for(int j = 0; j < xp.Count(); j++)
                    {
                        Mark residualLine = new LineMark(xp[j], xp[j], yp[j], mp[j]).SetStroke(Color.Orange, 0.5);
                        pes.Add(residualLine);
                    }
                }
            }
            else if (displayType == DisplayType.MARGINAL_PREDICTED)
            {
                if (options.DrawEstimate)
                {
                    Symbol x = coplot.XSymbol(), y = coplot.YSymbol();
                    DataSet ds = dataSet.where(coplot.Levels.ToArray());
                    DataSource newX = ds.Evaluate(x.Formula);

                    List<double> mp = model.yValues(slice);
                    if (mp.Count() > 0)
                    {
                        mp = ds.RowNames.Select(s => (coplot.TrainingData as DataSet).RowNames.Contains(s) ?
                            mp[(coplot.TrainingData as DataSet).RowNames.IndexOf(s)] : double.PositiveInfinity).ToList();
                    }

                    var points = ds.Select<double>(x, y);

                    List<double> xp, yp;
                    xp = points[0];
                    yp = points[1];

                    for (int j = 0; j < xp.Count(); j++)
                    {
                        Mark residualLine = new DotMark(xp[j], mp[j]).SetStroke(Color.Orange, 0.5);
                        pes.Add(residualLine);
                    }
                }
            }
            panel.Children = pes;
        }

        public List<Mark> draw(Model.Slice slice, Color color, string label)
        {
            Symbol x = coplot.XSymbol(), y = coplot.YSymbol();

            List<Mark> result = new List<Mark>();

            List<double> xp = model.xValues(slice);
            List<double> yp = model.yValues(slice);

            List<double> rxp = xp.Where((d,i)=>slice.InData.Contains(i)).ToList();
            List<double> ryp = yp.Where((d,i)=>slice.InData.Contains(i)).ToList();

            List<double> rp = null, qp = null, rrp = null, rqp = null;
            if (options.DrawEstimateInterval)
            {
                rp = model.yValues(slice, (0.5 - options.EstimateInterval / 2));
                qp = model.yValues(slice, (0.5 + options.EstimateInterval / 2));

                rrp = rp.Where((d,i)=>slice.InData.Contains(i)).ToList();
                rqp = qp.Where((d,i)=>slice.InData.Contains(i)).ToList();
            }

            List<double> up = null, dp = null, rup = null, rdp = null;
            if (options.DrawPredictionInterval)
            {
                up = model.simValues(slice, (0.5 - options.PredictionInterval / 2));
                dp = model.simValues(slice, (0.5 + options.PredictionInterval / 2));
            
                rup = up.Where((d,i)=>slice.InData.Contains(i)).ToList();
                rdp = dp.Where((d,i)=>slice.InData.Contains(i)).ToList();
            }

            // Interactive items
            LabelMark estimateText = new LabelMark("", 0, 0, Color.Black, ContentAlignment.MiddleRight).SetVisible(false).SetOrder(1000).SetFillColor(Color.White);
            result.Add(estimateText);

            LabelMark predictorText = new LabelMark("", 0, 0, Color.Black, ContentAlignment.BottomLeft).SetVisible(false).SetOrder(1000).SetFillColor(Color.White);
            result.Add(predictorText);

            Pen estimateStroke = new Pen(Color.FromArgb(128, Color.Black), 0.5f);
            estimateStroke.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            LineMark estimateLine = new LineMark(0, 0, 0, 0).SetStroke(estimateStroke).SetVisible(false).SetOrder(999);
            result.Add(estimateLine);

            LabelMark upperText = new LabelMark("", 0, 0, Color.Black, ContentAlignment.BottomRight).SetVisible(false).SetOrder(1000).SetFillColor(Color.White);
            result.Add(upperText);

            LabelMark lowerText = new LabelMark("", 0, 0, Color.Black, ContentAlignment.TopRight).SetVisible(false).SetOrder(1000).SetFillColor(Color.White);
            result.Add(lowerText);

            if (x is Variable && yp.Count() > 1)
            {
                if (options.DrawEstimateInterval)
                {
                    result.Add(new AreaMark(xp, rp, xp, qp).SetFillColor(Color.Transparent).SetStrokeColor(Color.FromArgb(64, color))
                        .SetMouseOver(p =>
                        {
                            int index = closestIndex(xp, p.X);
                            upperText.SetPosition(xp[index], qp[index]).SetText(FormatNum(qp[index], y)).SetVisible(true);
                            lowerText.SetPosition(xp[index], rp[index]).SetText(FormatNum(rp[index], y)).SetVisible(true);
                            estimateText.SetPosition(xp[index], yp[index]).SetText(FormatNum(yp[index], y)).SetVisible(true);
                            predictorText.SetPosition(xp[index], coplot.YVisibleRange().min).SetText(FormatNum(xp[index], x)).SetVisible(true);
                            estimateLine.SetPosition(xp[index], xp[index], coplot.YVisibleRange().min, coplot.YVisibleRange().max).SetVisible(true);
                            return true;
                        })
                        .SetMouseExit(p => { upperText.SetVisible(false); lowerText.SetVisible(false); estimateText.SetVisible(false); predictorText.SetVisible(false);  estimateLine.SetVisible(false); return true; }));

                    result.Add(new AreaMark(rxp, rrp, rxp, rqp).SetFillColor(Color.FromArgb(64, color)).SetStrokeColor(Color.FromArgb(128, color)));
                }

                if (options.DrawPredictionInterval)
                {
                    result.Add(new AreaMark(xp, dp, xp, up)
                        .SetFillColor(Color.Transparent)
                        .SetStrokeColor(Color.FromArgb(64, color)));
                    
                    result.Add(new AreaMark(rxp, rdp, rxp, rup)
                        .SetFillColor(Color.Transparent)
                        .SetStrokeColor(Color.FromArgb(128, color))
                        .SetData("predictionInterval"));
                }

                if (options.DrawEstimate)
                {                    
                    result.Add(new LineMark(rxp, ryp).SetStrokeColor(Color.FromArgb(230, color))
                        .SetOrder(10).SetStrokeWidth(4));
                    //result.Add(new LineMark(xp, yp).SetStrokeColor(Color.FromArgb(96, color)).SetOrder(9));
                    result.Add(new LineMark(xp, yp).SetStroke(Color.Transparent, 15)
                        .SetOrder(9)
                        .SetMouseOver(p => { 
                            int index = closestIndex(xp, p.X);
                            estimateText.SetPosition(xp[index], yp[index]).SetText(FormatNum(yp[index], y)).SetVisible(true);
                            predictorText.SetPosition(xp[index], coplot.YVisibleRange().min).SetText(FormatNum(xp[index], x)).SetVisible(true);
                            estimateLine.SetPosition(xp[index], xp[index], coplot.YVisibleRange().min, coplot.YVisibleRange().max).SetVisible(true);
                            return true;})
                        .SetMouseExit(p => { estimateText.SetVisible(false); predictorText.SetVisible(false);  estimateLine.SetVisible(false); return true; }));
                    
                    double labelX, labelY;
                    labelY = yp.Last(d => (d < coplot.YVisibleRange().max && d > coplot.YVisibleRange().min));
                    labelX = xp.ElementAt(yp.LastIndexOf(labelY));

                    result.Add(new LabelMark(label, labelX, labelY, Color.Orange, ContentAlignment.BottomRight));
                }

                /*if (ShowXEstimate)
                {
                    int closestIndex =
                            xp.Select((n, i) => new { diff = Math.Abs(n - HoveredX), index = i })
                            .OrderBy(p => p.diff)
                            .First().index;

                    double r = (x is TransformedVariable) ? (x as TransformedVariable).evaluateInverse(xp[closestIndex]) : xp[closestIndex];
                    double m = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(yp[closestIndex]) : yp[closestIndex];

                    result.Add(new LineMark(
                                new List<double> { xp[closestIndex], xp[closestIndex] },
                                new List<double> { coplot.YVisibleRange().min, coplot.YVisibleRange().max }
                                ).SetStrokeColor(Color.FromArgb(128, Color.Black)).SetStrokeWidth(0.5f));

                    result.Add(new LabelMark(
                        FormatNum(r, x), xp[closestIndex], coplot.YVisibleRange().min, Color.Black, ContentAlignment.BottomLeft));
                    
                    if (options.DrawEstimate)
                    {                        
                        result.Add(new LabelMark(
                            FormatNum(m, y), xp[closestIndex], yp[closestIndex], Color.Black, ContentAlignment.MiddleRight));
                    }

                    if (options.DrawEstimateInterval)
                    {
                        result.Add(new LineMark(
                            new List<double> { xp[closestIndex], xp[closestIndex] },
                            new List<double> { qp[closestIndex], rp[closestIndex] }                           
                            ).SetStrokeColor(Color.White));

                        double l = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(qp[closestIndex]) : qp[closestIndex];
                        double h = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(rp[closestIndex]) : rp[closestIndex];
                        
                        result.Add(new LabelMark(
                            FormatNum(l, y), xp[closestIndex], qp[closestIndex], Color.Black, ContentAlignment.BottomRight));
                        result.Add(new LabelMark(
                            FormatNum(h, y), xp[closestIndex], rp[closestIndex], Color.Black, ContentAlignment.TopRight));
                    }

                    if (options.DrawPredictionInterval)
                    {
                        result.Add(new LineMark(
                            new List<double> { xp[closestIndex], xp[closestIndex] },
                            new List<double> { dp[closestIndex], up[closestIndex] }                            
                            ).SetStrokeColor(Color.DarkGray));

                        double l = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(dp[closestIndex]) : dp[closestIndex];
                        double h = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(up[closestIndex]) : up[closestIndex];

                        result.Add(new LabelMark(
                            FormatNum(l, y), xp[closestIndex], dp[closestIndex], Color.Black, ContentAlignment.BottomRight));
                        result.Add(new LabelMark(
                            FormatNum(h, y), xp[closestIndex], up[closestIndex], Color.Black, ContentAlignment.TopRight));
                    }
                }

                if (ShowYEstimate)
                {
                    double r = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(HoveredY) : HoveredY;

                    result.Add(new LineMark(
                                new List<double> { coplot.XVisibleRange().min, coplot.XVisibleRange().max },
                                new List<double> { HoveredY, HoveredY }                             
                                ).SetStrokeColor(Color.FromArgb(128, Color.Black)).SetStrokeWidth(0.5f));

                    result.Add(new LabelMark(
                            FormatNum(r, y), coplot.XVisibleRange().min, HoveredY, Color.Black, ContentAlignment.BottomLeft));


                    if (options.DrawEstimate)
                    {
                        int closestIndex =
                            yp.Select((n, i) => new { diff = Math.Abs(n - HoveredY), index = i })
                            .OrderBy(p => p.diff)
                            .First().index;

                        if (closestIndex > 0 && closestIndex < yp.Count() - 1)
                        {
                            double m = (x is TransformedVariable) ? (x as TransformedVariable).evaluateInverse(xp[closestIndex]) : xp[closestIndex];
                        
                            result.Add(new LabelMark(
                                FormatNum(m, x), xp[closestIndex], HoveredY, Color.Black, ContentAlignment.TopLeft));
                        }
                    }
                }*/
            }
            else if(yp.Count() == xp.Count())
            {
                for (int i = 0; i < xp.Count(); i++)
                {
                    double min = xp[i]-0.5;
                    double max = xp[i]+0.5;
                    
                    if (options.DrawPredictionInterval)
                    {
                        result.Add(new AreaMark(
                            new List<double> { min, max }, new List<double> { dp[i], dp[i] },
                            new List<double> { min, max }, new List<double> { up[i], up[i] })
                            .SetFillColor(Color.Transparent)
                            .SetStrokeColor(Color.FromArgb(64, color)));

                        if (slice.InData.Contains(i))
                        {
                            result.Add(new AreaMark(
                                new List<double> { min, max }, new List<double> { dp[i], dp[i] },
                                new List<double> { min, max }, new List<double> { up[i], up[i] }
                                ).SetFillColor(Color.FromArgb(128, color))
                                .SetStrokeColor(Color.FromArgb(128, color))
                                .SetData("predictionInterval"));
                        }
                    }

                    if (options.DrawEstimateInterval)
                    {
                        result.Add(new AreaMark(
                            new List<double> { min, max }, new List<double> { rp[i], rp[i] },
                            new List<double> { min, max }, new List<double> { qp[i], qp[i] })
                            .SetFillColor(Color.Transparent)
                            .SetStrokeColor(Color.FromArgb(64, color)));
                        
                        if (slice.InData.Contains(i))
                        {
                            result.Add(new AreaMark(
                                new List<double> { min, max }, new List<double> { rp[i], rp[i] },
                                new List<double> { min, max }, new List<double> { qp[i], qp[i] }
                                )
                                .SetFillColor(Color.FromArgb(64, color))
                                .SetStrokeColor(Color.FromArgb(128, color))
                                .SetData("estimateInterval"));
                        }
                    }

                    if (options.DrawEstimate)
                    {
                        result.Add(new LineMark(
                            new List<double> { min, max },
                            new List<double> { yp[i], yp[i] })
                            .SetStrokeColor(Color.FromArgb(96, color))
                            .SetOrder(9));

                        if (slice.InData.Contains(i))
                        {
                            result.Add(new LineMark(
                                new List<double> { min, max },
                                new List<double> { yp[i], yp[i] })
                                .SetStroke(Color.FromArgb(230, color), 2)
                                .SetOrder(10));
                        }

                        int j = i;
                        result.Add(new LineMark(
                            new List<double> { min, max },
                            new List<double> { yp[i], yp[i] })
                            .SetStrokeColor(Color.Transparent)
                            .SetStrokeWidth(12)
                            .SetOrder(9)
                            .SetMouseOver(p => { 
                                estimateText.SetPosition(p.X, yp[j]).SetText(FormatNum(yp[j], y)).SetVisible(true);
                                estimateLine.SetPosition(p.X, p.X, coplot.YVisibleRange().min, coplot.YVisibleRange().max).SetVisible(true);
                                return true;})
                            .SetMouseExit(p => { estimateText.SetVisible(false); estimateLine.SetVisible(false); return true; }));
                    }

                    /*if (ShowXEstimate && HoveredX >= min && HoveredX <= max)
                    {
                        if (options.DrawEstimate)
                        {
                            // should show value here for consistency
                            double m = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(yp[i]) : yp[i];

                            result.Add(new LabelMark(
                                FormatNum(m, y), xp[i], yp[i], Color.Black, ContentAlignment.MiddleRight));
                        }

                        if (options.DrawEstimateInterval)
                        {
                            result.Add(new LineMark(
                                new List<double> { xp[i], xp[i] },
                                new List<double> { qp[i], rp[i] }                               
                                ).SetStrokeColor(Color.White));
                            
                            double l = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(qp[i]) : qp[i];
                            double h = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(rp[i]) : rp[i];
                            double m = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(yp[i]) : yp[i];

                            result.Add(new LabelMark(
                                FormatNum(l, h - l), xp[i], qp[i], Color.Black, ContentAlignment.BottomRight));
                            result.Add(new LabelMark(
                                FormatNum(h, h - l), xp[i], rp[i], Color.Black, ContentAlignment.TopRight));

                        }

                        if (options.DrawPredictionInterval)
                        {
                            result.Add(new LineMark(
                                new List<double> { xp[i], xp[i] },
                                new List<double> { dp[i], up[i] }
                                ).SetStrokeColor(Color.White));

                            double l = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(dp[i]) : dp[i];
                            double h = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(up[i]) : up[i];
                            double m = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(yp[i]) : yp[i];

                            result.Add(new LabelMark(
                                FormatNum(l, h - l), xp[i], dp[i], Color.Black, ContentAlignment.BottomRight));
                            result.Add(new LabelMark(
                                FormatNum(h, h - l), xp[i], up[i], Color.Black, ContentAlignment.TopRight));
                        }
                    }*/
                }

                /*if (ShowYEstimate)
                {
                    double m = (y is TransformedVariable) ? (y as TransformedVariable).evaluateInverse(HoveredY) : HoveredY;

                    result.Add(new LineMark(
                                new List<double> { coplot.XVisibleRange().min, coplot.XVisibleRange().max },
                                new List<double> { HoveredY, HoveredY }                     
                                ).SetStrokeColor(Color.FromArgb(128, Color.Black)).SetStrokeWidth(0.5f));

                    result.Add(new LabelMark(
                            FormatNum(m, y), coplot.XVisibleRange().min, HoveredY, Color.Black, ContentAlignment.BottomLeft));
                }*/
            }
            

            // add axis rectangles for brushing...
            //Mark bottomRect = new AreaMark(new double[] { 0, 1 }, new double[] { 40, 40 }, new double[] { 0, 1 }, new double[] { 0, 0 }, Color.Transparent).Payload("BottomAxis");
            //bottomRect.Transform(BottomMarginTransform);
            //result.Add(bottomRect);

            //Mark leftRect = new AreaMark(new double[] { -100, 0 }, new double[] { 0, 0 }, new double[] { -100, 0 }, new double[] { 1, 1 }, Color.Transparent).Payload("LeftAxis");
            //leftRect.Transform(LeftMarginTransform);
            //result.Add(leftRect);

            return result;
        }

        private int closestIndex(IEnumerable<double> xp, double x)
        {
            return xp.Select((n, i) => new { diff = Math.Abs(n - x), index = i })
                    .OrderBy(p => p.diff)
                    .First().index;
        }

        protected bool OnMouseOverHandler(Point point, Rectangle rec, IEnumerable<Mark> payloads)
        {
            var strings = from p in payloads select Convert.ToString(p.Data);

            if (strings.Contains("estimate"))
            {
                //HoveredX = payloads.First(e => (e.Data as string) == "estimate").Transformation.untransform(point, rec).X;
                ShowXEstimate = true;
                ShowYEstimate = false;
            }

            if (strings.Contains("estimateInterval"))
            {
                //HoveredX = payloads.First(e => (e.Data as string) == "estimateInterval").Transformation.untransform(point, rec).X;
                ShowXEstimate = true;
                ShowYEstimate = false;
            }

            if (strings.Contains("predictionInterval"))
            {
                //HoveredX = payloads.First(e => (e.Data as string) == "predictionInterval").Transformation.untransform(point, rec).X;
                ShowXEstimate = true;
                ShowYEstimate = false;
            }

            if (strings.Contains("BottomAxis"))
            {
                //HoveredX = this.coplot.XVisibleRange().unmap(payloads.First(e => (e.Data as string) == "BottomAxis").Transformation.untransform(point, rec).X);
                ShowXEstimate = true;
                ShowYEstimate = false;
            }

            if (strings.Contains("LeftAxis"))
            {
                //HoveredY = this.coplot.YVisibleRange().unmap(payloads.First(e => (e.Data as string) == "LeftAxis").Transformation.untransform(point, rec).Y);
                ShowYEstimate = true;
                ShowXEstimate = false;
            }

            return payloads.Count() > 0;
        }

        protected bool OnMouseLeaveHandler(Point point, Rectangle rec, IEnumerable<Mark> payloads)
        {
            ShowXEstimate = false;
            ShowYEstimate = false;
            return true;
        }

        protected ToolStripItem[] OnContextMenuHandler(Point point, Rectangle rec, IEnumerable<Mark> payloads)
        {
            //IEnumerable<string> over = from p in payloads select Convert.ToString(p);

            List<ToolStripItem> result = new List<ToolStripItem>();

            /*if (over.Contains("estimate"))
            {
                ToolStripMenuItem item = new ToolStripMenuItem("Add interaction");
                foreach (Term t in model.GetPossibleInteractions())
                {
                    Term s = t;
                    ToolStripMenuItem inter = new ToolStripMenuItem(t.PrettyName, null,
                        delegate(object sender, EventArgs args)
                        {
                            model.Predictors.Add(s);
                            model.fit();
                            modeler.updateModel(model);
                            modeler.NotifyViewChanged();
                        });
                    item.DropDownItems.Add(inter);
                }

                result.Add(item);
            }*/

            return result.ToArray();
        }

        /*public override List<double> residuals(Nest x, Nest y, Nest g, PlotOptions options)
        {
            {
                Term t = x.Variable;

                int factorLevel = -1;
                if (f.Response is Factor)
                {
                    foreach (Level l in y.Levels)
                    {
                        if (l.Factor == f.Response)
                            factorLevel = l.LevelIndex;
                    }

                    foreach (Level l in g.Levels)
                    {
                        if (l.Factor == f.Response)
                            factorLevel = l.LevelIndex;
                    }
                }

                int idx = 0;

                // not reentrant, new simulation will destroy old one...
                string setx = internalName + "." + idx + ".x";
                string sim = internalName + "." + idx + ".s";

                string query = "with(with(" + dataSet + "," + dataSet +
                    (parameters.Count() == 0 ? "" :
                        "[" + string.Join("&", (from p in parameters select "(" + p.Clause() + ")").ToArray()) + ",]")
                        + "), as.vector(" + x.range(fitPoints) + "))";

            IEnumerable<string> par =
                (new List<string> { internalName + "." + idx, x.Name + "=" + query })
                .Concat(from p in parameters select p.Factor.Name + "=with(" + dataSet + ", " + p.Formula() + ")");

            RSource.Instance.setRValue(setx, "setx(" + string.Join(", ", par.ToArray()) + ")");
            RSource.Instance.setRValue(sim, "sim(" + internalName + "." + idx + ", x=" + setx + ")");
            return sim;

            string simulation = simulate(i, x.Variable, x.Levels.Union(y.Levels).Union(g.Levels).ToArray());

            List<double> xp = parameterValues(x.Variable, fitPoints, x.Levels.Union(y.Levels).Union(g.Levels).ToArray());
            List<double> yp = evaluate(simulation, "ev", "mean", factorLevel);
        }*/
        
        /*protected override Factor.Level[] necessaryConditions(Factor.Level[] conditions)
        {
            NameComparer nc = new NameComparer();

            return (from c in conditions where model.Predictors.Contains(c.Factor, nc) ||
                   nc.Equals(model.Response, c.Factor)
                   select c).ToArray();
        }*/
    }
}
