using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Language;
using System.Diagnostics;
using Layout.Formatters;

namespace Layout
{
    public class AxisLayout
    {
        public enum Algorithm { OURS, WILKINSON, HECKBERT, MATPLOTLIB };
        public static Algorithm algorithm = Algorithm.OURS;

        public static double AxisDensity = 1.0/150;
        public static double AxisFontSize = 12.0; 

        public AxisLabeler.Options options;

        public AxisLayout(bool yAxis, Vector symbol, Range dataRange, Range visibleRange, Func<string, decimal, Axis, RectangleF> ComputeLabelRect, RectangleF screen)
        {
            this.options = new AxisLabeler.Options();
            this.options.direction = yAxis ? Axis.Direction.VERTICAL : Axis.Direction.HORIZONTAL;
            this.options.symbol = symbol;
            this.options.dataRange = dataRange;
            this.options.visibleRange = visibleRange;
            this.options.fontSize = (int)AxisFontSize;
            this.options.ComputeLabelRect = ComputeLabelRect;
            this.options.screen = screen;
        }

        public Axis layoutAxis()
        {
            AxisLabeler labeler = new NoOpAxisLabeler();

            if (options.symbol is Numeric)
            {
                switch(algorithm) 
                {
                    case Algorithm.OURS:        labeler = new ExtendedAxisLabeler(); break;
                    case Algorithm.HECKBERT:    labeler = new HeckbertAxisLabeler(); break;
                    case Algorithm.MATPLOTLIB:  labeler = new MatplotlibAxisLabeler(); break;
                    case Algorithm.WILKINSON:   labeler = new WilkinsonAxisLabeler(); break;
                }
            }
            else if (options.symbol is Factor)
            {
                labeler = new CategoricalAxisLabeler();
            }

            return labeler.generate(options, AxisDensity);
        }
    }
}
