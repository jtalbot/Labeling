using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;

namespace Layout.Formatters
{
    public abstract class Format
    {
        protected double weight;

        public Format(double weight)
        {
            this.weight = weight;
        }

        public abstract double Score(IEnumerable<Object> val);

        public abstract Tuple<IEnumerable<String>, String> FormalLabels(IEnumerable<Object> o);
    }

    public abstract class NumericFormat : Format
    {        
        protected bool factored;        // if true, 10^power portion will be placed on the axis title
        protected bool decimalExtend;   // if true, labels will be extended to the same number of decimal places

        public NumericFormat(bool factored, bool decimalExtend, double weight) : base(weight)
        {
            this.factored = factored;
            this.decimalExtend = decimalExtend;
        }

        public override double Score(IEnumerable<Object> val)
        {
            return 0.9 * val.Select(x => (decimal)x == 0 ? 1 : weight*Score((decimal)x)).Average() + 0.1 * (decimalExtend ? 1 : 0);
        }

        public abstract double Score(decimal d);

        public override Tuple<IEnumerable<string>, string> FormalLabels(IEnumerable<Object> o)
        {
            return FormatLabels(o.Cast<decimal>());
        }

        public abstract Tuple<IEnumerable<string>, string> FormatLabels(IEnumerable<decimal> d);

        protected int pot(decimal val) 
        {
            return (int)Math.Floor(Math.Log10((double)Math.Abs(val)));
        }

        protected decimal pow10(int i)
        {
            decimal a = 1m;
            for (int j = 0; j < i; j++) a *= 10m;
            return a;
        }

        protected int decimalPlaces(decimal i)
        {
            string t = i.ToString("G29");
            int s = t.IndexOf(".");
            if (s < 0) return 0;
            else return (t.Length - (s+1));
        }
    }

    public class UnitFormat : NumericFormat
    {
        decimal unit;
        string name;
        Range potRange;

        public UnitFormat(decimal unit, string name, Range potRange, bool factored, bool decimalExtend, double weight)
            : base(factored, decimalExtend, weight)
        {
            this.unit = unit;
            this.name = name;
            this.potRange = potRange;
        }

        public override double Score(decimal d)
        {
            return (pot(d) >= potRange.min && pot(d) <= potRange.max) ? 1 : 0;
        }

        public override Tuple<IEnumerable<string>, string> FormatLabels(IEnumerable<decimal> d)
        {
            IEnumerable<decimal> r = from x in d select x / unit;
            int decimals = (from x in r select decimalPlaces(x)).Max();
            return new Tuple<IEnumerable<string>,string>(from x in r select x.ToString(decimalExtend ? "N" + decimals : "G29") + (factored ? "" : name), (factored ? name : ""));
        }
    }

    public class ScientificFormat : NumericFormat
    {
        public ScientificFormat(bool factored, bool decimalExtend, double weight)
            : base(factored, decimalExtend, weight)
        {
        }

        //scientific format for general numbers
        public override double Score(decimal d)
        {
            return 1;
        }

        public override Tuple<IEnumerable<string>, string> FormatLabels(IEnumerable<decimal> d)
        {
            int avgpot = (int)Math.Round((from x in d.Where(x=>x!=0) select pot(x)).Average());
            decimal s = pow10(avgpot);
            IEnumerable<decimal> r = from x in d select x / s;
            int decimals = (from x in r select decimalPlaces(x)).Max();
            string label = "x10\\^" + avgpot + "\\^";
            return new Tuple<IEnumerable<string>,string>(from x in r select x.ToString(decimalExtend ? "N" + decimals : "0.#") + (factored ? "" : label), (factored ? label : ""));
        }
    }
}