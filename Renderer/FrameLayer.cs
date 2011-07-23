using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;
using Data;
using System.Drawing;
using System.Windows.Forms;
using Layout;
using Layout.Formatters;

namespace Renderer
{
    public class FrameLayer : Layer
    {
        Panel panel, leftPanel, bottomPanel;

        public bool DrawAxisTitles = true;
        public bool DrawXAxis = true;
        public bool DrawYAxis = true;
        public bool DrawGridLines = true;
        public bool DrawLabels = true;

        public double Margin = 6;

        public FrameLayer(PlotPanel plotPanel)
            : base(plotPanel)
        {
            panel = new Panel().SetName("frame panel");
            plotPanel.Plot.Add(panel);
            leftPanel = new Panel();
            plotPanel.Left.Add(leftPanel);
            bottomPanel = new Panel();
            plotPanel.Bottom.Add(bottomPanel);
        }

        public override void Layout(Rectangle screen)
        {
            plotPanel.setScreen(screen);
            leftPanel.Children.Clear();
            bottomPanel.Children.Clear();
            panel.Children.Clear();

            Vector X = state.Frame.P.Columns[0];
            Vector Y = state.Frame.R.Columns[0];

            Range xRange = X.Range;
            Range yRange = Y.Range;

            // expand visible range by a fixed number of pixels...
            if (X is Numeric)
                xRange = X.Range.expand(Margin * (X.Range.size / (screen.Width - 2 * Margin)));
            if (Y is Numeric)
                yRange = Y.Range.expand(Margin * (Y.Range.size / (screen.Height - 2 * Margin)));
            
            // do axis layout
            AxisLayout xAxisLayout = new AxisLayout(false, X, X.Range, xRange,
                new Func<string, decimal, Axis, RectangleF>((label, pos, axis) => ComputeLabelRect(label, pos, bottomPanel.ToScreen(), screen, axis)), screen);
            Axis xAxis = xAxisLayout.layoutAxis();
            xRange = xAxis.visibleRange;
            
            AxisLayout yAxisLayout = new AxisLayout(true, Y, Y.Range, yRange,
                new Func<string, decimal, Axis, RectangleF>((label, pos, axis) => ComputeLabelRect(label, pos, leftPanel.ToScreen(), screen, axis)), screen);
            Axis yAxis = yAxisLayout.layoutAxis();
            yRange = yAxis.visibleRange;

            // The independent layouts of the x and y axes might result in different font sizes,
            // force them to be the same here.
            if (DrawXAxis && DrawYAxis)
                xAxis.fontSize = yAxis.fontSize = Math.Min(xAxis.fontSize, yAxis.fontSize);

            if (DrawXAxis) state.xVisibleRange = xRange;
            if (DrawYAxis) state.yVisibleRange = yRange;

            RectangleF bottomPanelSize = new LineMark(xRange.min, xRange.max, 0, 0).bounds(bottomPanel.ToScreen());
            RectangleF leftPanelSize = new LineMark(0, 0, yRange.min, yRange.max).bounds(leftPanel.ToScreen());

            // *** X Axis ***
            if (DrawXAxis)
            {
                foreach (Tuple<decimal, string> kvp in xAxis.labels)
                {
                    double value = (double)kvp.Item1;
                    bottomPanel.Add(new LineMark(value, value, 0, -xAxis.tickSize).SetStrokeColor(Color.Black).SetStrokeWidth(0.5)); // the tick mark

                    if (DrawLabels)
                        bottomPanel.Add(new LabelMark(kvp.Item2, value, -xAxis.tickSize, Color.Black, ContentAlignment.TopCenter, true)
                        .FontSize(xAxis.fontSize)
                        .SetHorizontal(xAxis.labelDirection == Axis.Direction.HORIZONTAL)); 

                    if (DrawGridLines)
                        panel.Add(new LineMark(value, value, yRange.min, yRange.max).SetStrokeColor(Color.FromArgb(200, 200, 200)).SetStrokeWidth(0.5));
                }

                bottomPanelSize = bottomPanel.Children.Aggregate(bottomPanelSize, (a, b) => a.Union(b.bounds(bottomPanel.ToScreen())));

                if (DrawAxisTitles)
                {
                    string axisTitle = X is Constant ? "" : state.Frame.P.Symbols[0].Name;
                    if (xAxis.axisTitleExtension != "")
                    {
                        axisTitle += String.Concat(" (", xAxis.axisTitleExtension, ")");
                    }
                    bottomPanel.Add(new LabelMark(axisTitle, (xRange.min + xRange.max) * 0.5, -(bottomPanelSize.Height + (DrawXAxis ? 8 : 0)), Color.Black, ContentAlignment.TopCenter, true).FontSize(xAxis.fontSize));
                    bottomPanelSize = bottomPanelSize.Union(bottomPanel.Children.Last().bounds(bottomPanel.ToScreen()));
                }
            }

            
            // *** Y Axis ***
            if (DrawYAxis)
            {
                foreach (Tuple<decimal, string> kvp in yAxis.labels)
                {
                    double value = (double)kvp.Item1;
                    leftPanel.Add(new LineMark(-yAxis.tickSize, 0, value, value).SetStrokeColor(Color.Black).SetStrokeWidth(0.5)); // the tick mark

                    if (DrawLabels)
                        leftPanel.Add(new LabelMark(kvp.Item2, -(yAxis.tickSize + 2), value, Color.Black, ContentAlignment.MiddleRight, true)
                            .FontSize(yAxis.fontSize)
                            .SetHorizontal(yAxis.labelDirection == Axis.Direction.HORIZONTAL));

                    if (DrawGridLines)
                        panel.Add(new LineMark(xRange.min, xRange.max, value, value).SetStrokeColor(Color.FromArgb(200, 200, 200)).SetStrokeWidth(0.5));
                }

                leftPanelSize = leftPanel.Children.Aggregate(leftPanelSize, (a, b) => a.Union(b.bounds(leftPanel.ToScreen())));

                if (DrawAxisTitles)
                {
                    string axisTitle = Y is Constant ? "" : state.Frame.R.Symbols[0].Name;
                    if (yAxis.axisTitleExtension != "")
                    {
                        axisTitle += String.Concat(" (\\*", yAxis.axisTitleExtension, "\\*)");
                    }
                    leftPanel.Add(new LabelMark(axisTitle, -(leftPanelSize.Width + (DrawYAxis ? 8 : 2)), (yRange.min + yRange.max) * 0.5, Color.Black, ContentAlignment.MiddleRight, true).FontSize(yAxis.fontSize).SetHorizontal(false));
                    leftPanelSize = leftPanelSize.Union(leftPanel.Children.Last().bounds(leftPanel.ToScreen()));
                }
            }

            // Expand margins to hold labels...
            state.CMarginLeft = Math.Max(state.CMarginLeft, Math.Max(leftPanelSize.Width, 0-bottomPanelSize.Left));
            state.CMarginBottom = Math.Max(state.CMarginBottom, Math.Max(bottomPanelSize.Height, leftPanelSize.Bottom - screen.Bottom));
            state.CMarginTop = Math.Max(state.CMarginTop, Math.Max(0, screen.Top - leftPanelSize.Top));
            state.CMarginRight = Math.Max(state.CMarginRight, Math.Max(0, bottomPanelSize.Right - screen.Right));
        }

        static public RectangleF ComputeLabelRect(string label, decimal position, Transform transform, Rectangle screen, Axis data)
        {
            LabelMark text;
            if (data.direction == Axis.Direction.VERTICAL)
            {
                ContentAlignment alignment = ContentAlignment.MiddleRight;
                text = new LabelMark(label, -data.tickSize, (double)position, Color.LightGray, alignment).FontSize(data.fontSize).SetHorizontal(data.labelDirection == Axis.Direction.HORIZONTAL);
            }
            else
            {
                ContentAlignment alignment = ContentAlignment.TopCenter;
                text = new LabelMark(label, (double)position, data.tickSize, Color.LightGray, alignment).FontSize(data.fontSize).SetHorizontal(data.labelDirection == Axis.Direction.HORIZONTAL);
            }

            return text.bounds(transform);
        }
    }
}
