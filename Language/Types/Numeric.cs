using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public class Numeric : Vector
    {
        public List<double> vector;

        public override int Length
        {
            get { return vector.Count(); }
        }

        
        public enum NumericDomain { COUNT, RATIO, AMOUNT, BALANCE, TEMPORAL };
    
        NumericDomain domain;
        public NumericDomain Domain { get { return domain; } }

        Range range;
        public override Range Range { get { return range; } }

        public override string Type { get { return new System.Globalization.CultureInfo("en").TextInfo.ToTitleCase(this.Domain.ToString().ToLower()); } }
        
        //public SType labelType;
        public int numUniqueValues;

        public Numeric(List<double> vector, NumericDomain domain)
        {
            this.vector = vector;
            this.domain = domain;
            this.range = new Range(vector.Min(), vector.Max());
            this.numUniqueValues = vector.Distinct().Count();
        }

        public Numeric(List<double> vector, NumericDomain domain, Range range, int numUniqueValues/*, Language.Types.SType type*/)
        {
            this.vector = vector;
            this.domain = domain;
            this.range = range;
            this.numUniqueValues = numUniqueValues;
            //this.labelType = type;
        }

        Dictionary<int, Factor> partitionedFactors = new Dictionary<int, Factor>();
        public Factor partition(int levels)
        {
            /*if(!partitionedFactors.ContainsKey(levels))
            {
                List<string> names = new List<string>();
                List<string> formulas = new List<string>();
                List<string> clauses = new List<string>();
                for (int i = 0; i < levels; i++)
                {
                    names.Add(((i+0.5)/ levels*100).ToString("0") + "%");
                    //names.Add(name + " ("+(i+1)+"/"+levels+")");
                    formulas.Add("quantile(" + name + "," + ((double)i + 0.5) / levels + ", type=1)");
                    clauses.Add("in.quantile(" + name + "," + ((double)i / levels) + "," + ((double)(i+1) / levels) + ")");
                }
                //partitionedFactors[levels] = new Factor(name, formula, this.Source, true, names, formulas, clauses);

                var quantiles = Enumerable.Range(0, levels+1).Select(l => ((double)l/levels).ToString());
                string quantile = "quantile(" + name + ", c(" + string.Join(",", quantiles.ToArray()) + "))";

                partitionedFactors[levels] = new Factor(name, "cut(" + name + ", " + quantile + ")", SymbolicSource.DATA, false, names, formulas, clauses);
            }
            return partitionedFactors[levels];*/

            throw new NotImplementedException();
        }

        // R function for selecting evenly spaced points in the range of this variable (move elsewhere?)
        public virtual string samplePoints(int strata)
        {
            //return "seq(min(" + formula + ", na.rm=TRUE)" + ", max(" + formula + ", na.rm=TRUE), length=" + strata + ")";
            throw new NotImplementedException();
        }

        public override List<T> Select<T>()
        {
            return vector.Select(v => (T)Convert.ChangeType(v, typeof(T))).ToList();
        }
    }
}
