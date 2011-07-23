using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;

namespace Layout
{
    class HeckbertAxisLabeler : AxisLabeler
    {
        private double heckbertNiceNum(double x, bool round)
        {
            int exp = (int)Math.Log10(x);
            double f = x / (Math.Pow(10.0, exp));
            double nf = 1;

            if (round)
            {
                if (f < 1.5)
                    nf = 1;
                else if (f < 3)
                    nf = 2;
                else if (f < 7)
                    nf = 5;
                else
                    nf = 10;
            }
            else
            {
                if (f <= 1)
                    nf = 1;
                else if (f <= 2)
                    nf = 2;
                else if (f <= 5)
                    nf = 5;
                else
                    nf = 10;
            }
            return nf * Math.Pow(10.0, exp);

        }

        public override Axis generate(Options options, double density)
        {
            double m = ((options.direction == Axis.Direction.HORIZONTAL ? options.screen.Width : options.screen.Height) * density);
            m = Math.Max(m, 2);

            Numeric v = options.symbol as Numeric;
            
            //loose labeling
            double d = 0; //tick mark spacing
            double vMin = v.Range.min;
            double vMax = v.Range.max;
            double range = heckbertNiceNum(vMax - vMin, false);
            d = heckbertNiceNum(range / (m - 1), true);
            double rMin = Math.Floor(vMin / d) * d;
            double rMax = Math.Ceiling(vMax / d) * d;
            int nfrac = (int)Math.Max(-Math.Floor(Math.Log10(d)), 0);

            List<Tuple<decimal, string>> labels = new List<Tuple<decimal, string>>();
            Axis option = options.DefaultAxis();

            double currX = rMin;
            while (currX <= rMax + 0.5 * d)
            {
                labels.Add(new Tuple<decimal, string>((decimal)currX, currX.ToString()));
                currX += d;
            }

            option.visibleRange = new Range(rMin, currX - d);

            option.labels = labels;
            option.score = 1;

            return option;
        }
    }


}
