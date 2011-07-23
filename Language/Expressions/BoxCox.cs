using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public enum Transformation { POLY2, IDENTITY, SQRT, CBRT, LOG, INVERSE };
        
    public class BoxCox : Expression
    {
        Expression child;

        public override IEnumerable<Value> Eval(IEnumerable<Value> args)
        {
            foreach(Value v in child.Eval(args))
            {
                if (!(v is Frame))
                    throw new InvalidOperationException("BoxCox only works on frames");

                Frame f = v as Frame;

                Frame r = new Frame();
                for (int i = 0; i < f.Columns.Count(); i++)
                {
                    Vector vec = f.Columns[i];

                    if (!(vec is Numeric))
                        throw new InvalidOperationException("BoxCox only works on numeric variables");

                    Numeric n = vec as Numeric;

                    List<double> l = evaluate(n.vector).ToList();

                    r.Columns.Add(new Numeric(l, evaluate(n.Domain)));
                    r.Symbols.Add(new Symbol(MakeName(f.Symbols[i].Name)));
                }
                yield return r;
            }
        }

        Numeric variable = null;
        public Numeric Original { get { return variable; } }
        
        Transformation type;
        public Transformation Type { get { return type; } }

        private double start;

        public BoxCox(Expression child, Transformation type)
        {
            this.type = type;
            this.child = child;
            start = ((Type == Transformation.INVERSE || Type == Transformation.LOG || Type == Transformation.SQRT || Type == Transformation.CBRT) && Original.Range.min < 0.1) ? 0.1 - Original.Range.min : 0;
        }

        public string MakeName(string term)
        {
            switch (Type)
            {
                case Transformation.INVERSE:
                    return "-1/" + term;
                case Transformation.LOG:
                    return "log " + term;
                case Transformation.CBRT:
                    return "3√" + term;
                case Transformation.SQRT:
                    return "√" + term;
                case Transformation.IDENTITY:
                    return term;
                case Transformation.POLY2:
                    return term + "²";
                default:
                    return term;
            }
        }

        public IEnumerable<double> evaluate(IEnumerable<double> v)
        {
            switch (Type)
            {
                case Transformation.INVERSE:
                    return from d in v select -1.0/(d+start);
                case Transformation.LOG:
                    return from d in v select Math.Log10(d+start);
                case Transformation.CBRT:
                    return from d in v select Math.Pow(d + start, (1.0/3));
                case Transformation.SQRT:
                    return from d in v select Math.Sqrt(d+start);
                case Transformation.IDENTITY:
                    return v;
                case Transformation.POLY2:
                    return from d in v select d*d;
                default:
                    return v;
            }
        }

        public IEnumerable<double> evaluateInverse(IEnumerable<double> v)
        {
            switch (Type)
            {
                case Transformation.INVERSE:
                    return from d in v select (-1.0 / d) - start;
                case Transformation.LOG:
                    return from d in v select Math.Pow(10,d) - start;
                case Transformation.CBRT:
                    return from d in v select d * d * d - start;
                case Transformation.SQRT:
                    return from d in v select d*d - start;
                case Transformation.IDENTITY:
                    return v;
                case Transformation.POLY2:
                    return from d in v select Math.Sqrt(d);
                default:
                    return v;
            }
        }

        public Numeric.NumericDomain evaluate(Numeric.NumericDomain d)
        {
            switch (Type)
            {
                case Transformation.INVERSE:
                    return Numeric.NumericDomain.BALANCE;
                case Transformation.LOG:
                    return Numeric.NumericDomain.BALANCE;
                case Transformation.CBRT:
                    return d;
                case Transformation.SQRT:
                    return d;
                case Transformation.IDENTITY:
                    return d;
                case Transformation.POLY2:
                    return d;   
                default:
                    return d;
            }            
        }

        public override string ToString()
        {
            return "BoxCox";
        }

        // pick evenly sampled points in this transformation, but in the space of the original variable
        /*
        public override string samplePoints(int strata)
        {
            return MakeInverseFormula("seq(min(" + ModelFormula + ", na.rm=TRUE)" + ", max(" + ModelFormula + ", na.rm=TRUE), length=" + strata + ")");
        }

        public override double Derivative(double x)
        {
            switch (Type)
            {
                case Transformation.INVERSE:
                    return Math.Pow(x, -2);
                case Transformation.LOG:
                    return 1.0 / (x + start);
                case Transformation.CBRT:
                    return 1.0/3 * (Math.Pow(x, -(2.0/3)));
                case Transformation.SQRT:
                    return 0.5 * (Math.Pow(x, -0.5));
                case Transformation.IDENTITY:
                    return 1;
                case Transformation.POLY2:
                    return 2*x;
                default:
                    return 1;
            }
        }
         * */
    }
}
