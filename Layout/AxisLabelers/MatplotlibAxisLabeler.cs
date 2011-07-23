using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;

namespace Layout
{
    class MatplotlibAxisLabeler : AxisLabeler
    {
        public override Axis generate(Options options, double density)
        {
            //int nbins = 9;      // in the actual Matplotlib implementation this was fixed at 9. here we let it vary like the other methods for a better comparison
            double m = ((options.direction == Axis.Direction.HORIZONTAL ? options.screen.Width : options.screen.Height) * density);
            int nbins = (int)Math.Max(m, 2);

            List<Axis> possibilities = new List<Axis>();
            List<double> steps = new List<double>() { 1, 2, 5, 10 };            
            bool trim = true;

            Numeric v = options.symbol as Numeric;
            double vMin = options.dataRange.min;
            double vMax = options.dataRange.max;

            Tuple<double, double> value = matPlotLibScaleRange(vMin, vMax, nbins);
            double scale = value.Item1;
            double offset = value.Item2;

            vMin -= offset;
            vMax -= offset;

            double rawStep = (vMax - vMin) / nbins;
            double scaledRawStep = rawStep / scale;

            double bestMax = vMax;
            double bestMin = vMin;

            double scaledStep = 1;
            foreach (int step in steps)
            {
                if (step < scaledRawStep)
                    continue;
                scaledStep = step * scale;
                bestMin = scaledStep * Math.Floor(vMin / scaledStep);
                bestMax = bestMin + scaledStep * nbins;
                if (bestMax >= vMax)
                    break;
            }
            if (trim)
            {
                int extraBins = (int)Math.Floor((bestMax - vMax) / scaledStep);
                nbins -= extraBins;
            }

            List<Tuple<decimal, string>> labels = new List<Tuple<decimal, string>>();
            Axis option = options.DefaultAxis();

            //compute actual labels
            for (int i = 0; i <= nbins; i++)
            {
                double labelVal = bestMin + i * scaledStep + offset;
                labels.Add(new Tuple<decimal, string>((decimal)labelVal, labelVal.ToString()));
            }
            option.labels = labels;
            option.score = 1;

            option.visibleRange = new Range(bestMin + offset, bestMin + nbins * scaledStep + offset);

            return option;
        }

        private Tuple<double, double> matPlotLibScaleRange(double min, double max, int bins, int threshold = 100)
        {
            double dv = Math.Abs(max - min);
            double maxabsv = Math.Max(Math.Abs(min), Math.Abs(max));
            double epsilon = Math.Pow(10, -12);
            if (maxabsv == 0 || dv / maxabsv < epsilon)
                return new Tuple<double, double>(1.0, 0.0);
            double meanv = 0.5 * (max + min);
            double offset = 0;
            double scale = 1;
            double exp = 1;

            if (Math.Abs(meanv) / dv < threshold)
                offset = 0;
            else if (meanv > 0)
            {
                exp = Math.Floor(Math.Log10(meanv));
                offset = Math.Pow(10.0, exp);
            }
            else
            {
                exp = Math.Floor(Math.Log10(-1 * meanv));
                offset = Math.Pow(-10.0, exp);
            }
            exp = Math.Floor(Math.Log10(dv / bins));
            scale = Math.Pow(10.0, exp);

            return new Tuple<double, double>(scale, offset);

        }

    }

}
