using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public abstract class Operation : Dictionary<Operator, bool>
    {
        public virtual string Name { get; set; }
        public Operator Default { get; set; }

        public virtual Func<Environment, double> OrderFunc(object obj)
        {
            return (cp => 0);
        }

        public bool isConstant()
        {
            return this.Select(kvp => kvp.Value).Count() <= 1;
        }
    }

    public static class OperationUtilities
    {
        public static IEnumerable<IEnumerable<Operator>> FullDesign(this IEnumerable<Operation> list)
        {
            if(list.Count() > 0)
            {
                //IEnumerable<Operator> levels = (constraints.Any(c=>c.Operation==list.First()) ? constraints.Where(c=>c.Operation==list.First()) : list.First().Operators(list.Count()==1));
                IEnumerable<Operator> levels = list.First().Where(kvp => kvp.Value).Select(p => p.Key);
                foreach (Operator c in  levels)
                {
                    foreach (IEnumerable<Operator> colevels in FullDesign(list.Skip(1)))
                    {
                        yield return new List<Operator> { c }.Concat(colevels);
                    }
                }
            }
            else
            {
                yield return Enumerable.Empty<Operator>();
            }
        }

        public static IEnumerable<IEnumerable<Operator>> NestedDesign(this IEnumerable<Operation> list)
        {
            if (list.Count() > 0)
            {
                //IEnumerable<Operator> levels = (constraints.Any(c => c.Operation == list.First()) ? constraints.Where(c => c.Operation == list.First()) : list.First().Operators(list.Count() == 1));
                IEnumerable<Operator> levels = list.First().Where(kvp => kvp.Value).Select(p => p.Key);
                foreach (Operator c in levels)
                {
                    foreach (IEnumerable<Operator> colevels in NestedDesign(list.Skip(1)))
                    {
                        yield return new List<Operator> { c }.Concat(colevels);
                    }

                    yield return new List<Operator> { c };
                }
            }
            else
            {
                yield return Enumerable.Empty<Operator>();
            }
        }

        /*public static int FullDesignSize(this IList<Conditioner> list)
        {
            if (list.Count() == 0)
                return 0;

            int result = 1;
            foreach (Conditioner c in list)
            {
                result *= c.Levels.Count();
            }
            return result;
        }*/
    }
}
