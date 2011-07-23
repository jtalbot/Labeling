using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public class Interaction
    {
        string name, formula;
        List<Term> terms;
        public List<Term> Terms { get { return terms; } }

        public Interaction(List<Term> terms)
        {
            this.terms = terms;
            this.name = String.Join("*", (from t in terms select t.Name).ToArray());
            this.formula = "(" + String.Join("*", (from t in terms select t.Formula).ToArray()) + ")";
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Interaction p = obj as Interaction;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return terms.Count() == p.Terms.Count() &&
                terms.Intersect(p.Terms).Count() == terms.Count();
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach (Term t in terms)
            {
                result = result ^ t.GetHashCode();
            }
            return result;
        }

        public Range VisibleRange
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double map(double d)
        {
            throw new NotImplementedException();
        }

        public double unmap(double d)
        {
            throw new NotImplementedException();
        }
    }
}
