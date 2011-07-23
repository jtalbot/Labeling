using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;
using Layout.Formatters;

namespace Layout
{
    /* Implements the axis labeling routine described in 
     *  Talbot, Lin, and Hanrahan. An Extension of Wilkinson’s Algorithm for Positioning Tick Labels on Axes, Infovis 2010.
     */
    class ExtendedAxisLabeler : AxisLabeler
    {
        List<decimal> Q = new List<decimal>() { 1m, 5m, 2m, 2.5m, 4m, 3m };
        List<double> w = new List<double>() { 0.25, 0.2, 0.5, 0.05 };
        List<Format> formats;

        QuantitativeFormatter formatter = new QuantitativeFormatter();

        void AddUnitFormat(decimal unit, string name, Range logRange, double weight, double factoredWeight)
        {
            formats.Add(new UnitFormat(unit, name, logRange, false, false, weight));
            formats.Add(new UnitFormat(unit, name, logRange, false, true, weight));
            formats.Add(new UnitFormat(unit, name, logRange, true, false, factoredWeight));
            formats.Add(new UnitFormat(unit, name, logRange, true, true, factoredWeight));
        }

        void AddUnitFormat(decimal unit, string name, Range logRange, double factoredWeight)
        {
            formats.Add(new UnitFormat(unit, name, logRange, true, false, factoredWeight));
            formats.Add(new UnitFormat(unit, name, logRange, true, true, factoredWeight));
        }

        public ExtendedAxisLabeler()
        {
            formats = new List<Format>();
            formats.Add(new UnitFormat(1m, "", new Range(-4, 6), false, false, 1));
            AddUnitFormat(1000m, "K", new Range(3, 6), 0.75, 0.4);
            AddUnitFormat(1000000m, "M", new Range(6, 9), 0.75, 0.4);
            AddUnitFormat(1000000000m, "B", new Range(9, 12), 0.75, 0.4);
            AddUnitFormat(100m, "hundred", new Range(2, 3), 0.35);
            AddUnitFormat(1000m, "thousand", new Range(3, 6), 0.5);
            AddUnitFormat(1000000m, "million", new Range(6, 9), 0.5);
            AddUnitFormat(1000000000m, "billion", new Range(9, 12), 0.5);
            AddUnitFormat(0.01m, "hundredth", new Range(-2, -3), 0.3);
            AddUnitFormat(0.001m, "thousandth", new Range(-3, -6), 0.5);
            AddUnitFormat(0.000001m, "millionth", new Range(-6, -9), 0.5);
            AddUnitFormat(0.000000001m, "billionth", new Range(-9, -12), 0.5);
            formats.Add(new ScientificFormat(true, false, 0.3));
            formats.Add(new ScientificFormat(true, true, 0.3));
            formats.Add(new ScientificFormat(false, false, 0.25));
            formats.Add(new ScientificFormat(false, true, 0.25));
        }

        protected decimal floored_mod(decimal a, decimal n)
        {
            return a - n * Math.Floor(a / n);
        }

        protected decimal pow10(int i)
        {
            decimal a = 1m;
            for (int j = 0; j < i; j++) a *= 10m;
            return a;
        }

        protected double simplicity(decimal q, List<decimal> Q, int j, decimal lmin, decimal lmax, decimal lstep)
        {
            decimal eps = 1e-10m;
            double n = Q.Count();
            double i = Q.IndexOf(q) + 1;
            double v = (floored_mod(lmin, lstep) < eps && lmin <= 0 && lmax >= 0) ? 1 : 0;
            if (n <= 1)
                return 1 - j + v;
            else
                return 1 - (i - 1) / (n - 1) - j + v;
        }

        protected double max_simplicity(decimal q, List<decimal> Q, int j)
        {
            double n = Q.Count();
            double i = Q.IndexOf(q) + 1;
            double v = 1;
            if (n == 1)
                return 1 - j + v;
            else
                return 1 - (i - 1) / (n - 1) - j + v;
        }

        protected double coverage(decimal dmin, decimal dmax, decimal lmin, decimal lmax)
        {
            return 1 - 0.5 * (double)(((dmax - lmax) * (dmax - lmax) + (dmin - lmin) * (dmin - lmin)) / ((0.1m * (dmax - dmin)) * (0.1m * (dmax - dmin))));
        }

        protected double max_coverage(decimal dmin, decimal dmax, decimal span)
        {
            decimal range = dmax - dmin;

            if (span > range)
            {
                decimal half = (span - range) / 2;
                return 1 - 0.5 * (double)((half*half + half*half) / ((0.1m * (dmax - dmin)) * (0.1m * (dmax - dmin))));
            }
            else
            {
                return 1;
            }
        }

        protected double density(double r, double rt)
        {
            return (2 - Math.Max(r / rt, rt / r));
        }

        protected double max_density(double r, double rt)
        {
            if (r >= rt)
                return 2 - r / rt;
            else
                return 1;
        }

        public override Axis generate(Options options, double density)
        {
            double space = (options.direction == Axis.Direction.HORIZONTAL ? options.screen.Width : options.screen.Height);

            Numeric v = options.symbol as Numeric;

            decimal dmax = (decimal)options.dataRange.max;
            decimal dmin = (decimal)options.dataRange.min;

            if (dmax == dmin)
                return null;

            Axis best = null;
            double bestScore = -2;

            int j = 1;
            while (j < int.MaxValue)
            {
                foreach (decimal q in Q)
                {
                    double sm = max_simplicity(q, Q, (int)j);
                    if (w[0] * sm + w[1] + w[2] + w[3] < bestScore)
                    {
                        j = int.MaxValue-1;
                        break;
                    }

                    int k = 2;
                    while (k < int.MaxValue)
                    {
                        double dm = max_density(k/space, density);

                        if (w[0] * sm + w[1] + w[2] * dm + w[3] < bestScore)
                            break;

                        decimal delta = (dmax - dmin) / (k + 1) / (j * q);
                        int z = (int)Math.Ceiling(Math.Log10((double)delta));

                        while (z < int.MaxValue)
                        {
                            decimal step = j * q * pow10(z);
                            double cm = max_coverage(dmin, dmax, step * (k - 1));

                            if (w[0] * sm + w[1] * cm + w[2] * dm + w[3] < bestScore)
                                break;

                            for (int start = (int)(Math.Floor(dmax / step - (k - 1)) * j); start <= (int)(Math.Ceiling(dmin / step)) * j; start++)
                            {
                                decimal lmin = start * step / j;
                                decimal lmax = lmin + step * (k - 1);

                                double s = simplicity(q, Q, (int)j, lmin, lmax, step);
                                double d = this.density(k/space, density);
                                double c = coverage(dmin, dmax, lmin, lmax);

                                if (w[0] * s + w[1] * c + w[2] * d + w[3] < bestScore)
                                    continue;

                                Axis option = options.DefaultAxis();

                                List<decimal> stepSequence = Enumerable.Range(0, (int)k).Select(x => lmin + x * step).ToList();
                                List<Tuple<decimal, string>> newlabels = stepSequence.Select(value => new Tuple<decimal, string>(value, value.ToString())).ToList();

                                option.labels = newlabels;
                                option.granularity = d;
                                option.coverage = c;
                                option.simplicity = s;
                                option.score = s + c + d;

                                //format and choose best
                                List<Axis> subPossibilities = new List<Axis>() { option };
                                Axis optionFormatted = formatter.format(
                                    formatter.varyOrientation(formatter.varyFontSize(subPossibilities, options)),
                                    formats,
                                    options,
                                    a => w[0] * a.simplicity + w[1] * a.coverage + w[2] * a.granularity + w[3] * a.legibility,
                                    bestScore);

                                double score = w[0] * optionFormatted.simplicity + w[1] * optionFormatted.coverage +
                                               w[2] * optionFormatted.granularity + w[3] * optionFormatted.legibility;

                                if (score > bestScore)
                                {
                                    bestScore = score;
                                    optionFormatted.score = score;
                                    best = optionFormatted;
                                }
                            }

                            z = z + 1;
                        }

                        k = k + 1;
                    }
                }

                j = j + 1;
            }

            if (best == null)
                Console.WriteLine("WARNING: Extended algorithm found 0 solutions");
            else
                best.visibleRange = new Range(Math.Min(options.visibleRange.min, (double)best.labels.Min(t => t.Item1)), Math.Max(options.visibleRange.max, (double)best.labels.Max(t => t.Item1)));
            return best;
        }
    }

}
