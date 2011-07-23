using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;

namespace Layout
{
    class WilkinsonAxisLabeler : AxisLabeler
    {
        public override Axis generate(Options options, double density)
        {
            return generate(options, density, 0.8);
        }

        public Axis generate(Options options, double density, double mincoverage)
        {
            double m = ((options.direction == Axis.Direction.HORIZONTAL ? options.screen.Width : options.screen.Height) * density);
            m = Math.Max(m, 2);

            Axis best = null;

            for (int i = 2; i < 12; i++)
            {
                Axis b = helper(options, i, mincoverage);
                double granularity = 1 - Math.Abs(i - m) / m;
                if (b != null && (best == null || b.score + granularity > best.score))
                {
                    best = b;
                    best.score += granularity;
                }
            }

            if (best != null)
            {
                best.visibleRange = new Range(Math.Min(options.visibleRange.min, (double)best.labels.Min(t => t.Item1)), Math.Max(options.visibleRange.max, (double)best.labels.Max(t => t.Item1)));
            }

            return best;
        }

        private Axis helper(Options options, double m, double mincoverage = 0.8)
        {
            double snice = double.NegativeInfinity;

            int intervals = (int)Math.Max(Math.Floor(m), 2) - 1;

            List<Axis> possibilities = new List<Axis>();

            Numeric v = options.symbol as Numeric;
            decimal min = (decimal)v.Range.min;
            decimal max = (decimal)v.Range.max;

            List<decimal> Q = new List<decimal> { 1m, 10m, 5m, 2m, 2.5m, 3m, 4m, 1.5m, 7m, 6m, 8m, 9m };
            int n = Q.Count();

            decimal range = max - min;
            decimal dc = range / intervals;
            decimal dbase = pow10((int)Math.Floor(Math.Log10((double)dc)));

            Axis best = null;

            foreach (decimal q in Q)
            {
                decimal tdelta = q * dbase;
                decimal tmin = Math.Floor(min / tdelta) * tdelta;
                decimal tmax = tmin + intervals * tdelta;

                int i = Q.IndexOf(q);
                double roundness = 1.0 - ((i + 1) - ((tmin <= 0 && tmax >= 0) ? 1.0 : 0.0)) / Q.Count();
                double coverage = (double)((max - min) / (tmax - tmin));

                if (coverage > mincoverage && tmin <= min && tmax >= max)
                {
                    double score = roundness + coverage;

                    if (score > snice)
                    {
                        List<decimal> stepSequence = Enumerable.Range(0, (int)intervals+1).Select(x => tmin + x * tdelta).ToList();
                        List<Tuple<decimal, string>> newlabels = stepSequence.Select(value => new Tuple<decimal, string>(value, value.ToString())).ToList();

                        Axis candidate = options.DefaultAxis();
                        candidate.score = score;
                        candidate.labels = newlabels;
                        best = candidate;

                        snice = score;
                    }
                }
            }

            return best;
        }

        protected decimal pow10(int i)
        {
            decimal a = 1m;
            for (int j = 0; j < i; j++) a *= 10m;
            return a;
        }
    }
}
