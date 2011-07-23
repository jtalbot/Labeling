using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Language
{
    public static class Utilities
    {
        /*public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>
            (this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            return ZipIterator(first, second, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>
            (IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> e1 = first.GetEnumerator())
            using (IEnumerator<TSecond> e2 = second.GetEnumerator())
                while (e1.MoveNext() && e2.MoveNext())
                    yield return resultSelector(e1.Current, e2.Current);
        }*/

        public static IEnumerable<TAccumulate> Accumulate<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (source == null)
                throw new ArgumentNullException("source", "Value cannot be null.");

            if (func == null)
                throw new ArgumentNullException("func", "Value cannot be null.");

            TAccumulate accumulator = seed;
            foreach (TSource item in source)
            {
                accumulator = func(accumulator, item);
                yield return accumulator;
            }
        }


        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException("list");
            if (items == null) throw new ArgumentNullException("items");
            if (list is BindingList<T>)
                (list as BindingList<T>).RaiseListChangedEvents = false;
            foreach (T item in items) list.Add(item);
            if (list is BindingList<T>)
            {
                (list as BindingList<T>).RaiseListChangedEvents = true;
                (list as BindingList<T>).ResetBindings();
            }
        }

        public static void InsertRange<T>(this IList<T> list, int i, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException("list");
            if (items == null) throw new ArgumentNullException("items");
            if (list is BindingList<T>)
                (list as BindingList<T>).RaiseListChangedEvents = false;
            foreach (T item in items)
            {
                list.Insert(i, item);
                i++;
            }

            if (list is BindingList<T>)
            {
                (list as BindingList<T>).RaiseListChangedEvents = true;
                (list as BindingList<T>).ResetBindings();
            }
        }

        public static int RemoveAll<T>(this IList<T> list, Func<T, bool> match)
        {
            if (list == null) throw new ArgumentNullException("list");
            if (list is BindingList<T>)
                (list as BindingList<T>).RaiseListChangedEvents = false;
            List<T> shouldRemove = list.Where(match).ToList();
            foreach (T t in shouldRemove)
                list.Remove(t);
            if (list is BindingList<T>)
            {
                (list as BindingList<T>).RaiseListChangedEvents = true;
                (list as BindingList<T>).ResetBindings();
            }
            return shouldRemove.Count();
        }

        public static bool ElementwiseEquals<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            if (a.Count() != b.Count())
                return false;

            return (from pair in a.Zip(b, (x, y) => new { first = x, second = y }) select pair.first.Equals(pair.second)).All(d=>d);
        }

        public static int GetElementwiseHashCode<T>(this IEnumerable<T> a)
        {
            int result = 0;
            foreach (T t in a)
            {
                result = result ^ t.GetHashCode();
            }
            return result;
        }

        /*
        // support hierarchical traversal of a list of terms
        // adds a constant variable on the end if needed...
        public static IEnumerable<Nest> ToDF(this IList<Term> oTerms)
        {
            List<Term> Terms = new List<Term>(oTerms);
            if (Terms.Count() == 0 || !(Terms.Last() is Variable))
                Terms.Add(Constant.Instance);

            List<Factor> factors = new List<Factor>();
            // don't traverse the last variable (constant or otherwise), special cased below...
            for (int i = 0; i < Terms.Count-1; i++ )
            {
                if (Terms[i] is Variable)
                    factors.Add((Terms[i] as Variable).partition(3));
                else if(Terms[i] is Factor)
                    factors.Add(Terms[i] as Factor);
            }

            // now traverse
            List<Level> parameters = new List<Level>();
            do
            {
                // move down if we can
                if (parameters.Count < factors.Count)
                {
                    parameters.Insert(0, factors[parameters.Count][0]);
                }
                // otherwise move to sibling
                else
                {
                    while (parameters.Count > 0)
                    {
                        parameters[0] = parameters[0].next();
                        if (parameters[0] != null)
                            break;
                        else
                            parameters.RemoveAt(0);
                    }
                }

                yield return new Nest(Terms, parameters.ToArray(), factors.Count - parameters.Count + 1);

                // Yield final, lowest Variable level if it exists...
                if (factors.Count == parameters.Count)
                {
                    yield return new Nest(Terms, parameters.ToArray(), (Terms[Terms.Count - 1] as Variable), 0);
                }
            } while (parameters.Count > 0);
        }

        public static Variable InnerVariable(this IList<Term> Terms)
        {
            return (Terms.Count > 0 && Terms[Terms.Count-1] is Variable) ? 
                Terms[Terms.Count-1] as Variable : 
                new Constant();
        }

        //walk down terms getting total number of items
        public static int PartitionCount(this IList<Term> Terms)
        {
            int cellCount = 1;
            foreach (Nest node in Terms.ToDF())
            {
                if (node.Height == 0)
                {
                    cellCount++;
                }
            }
            return cellCount;
        }
         * */
    }

    public class Range
    {
        public Range(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public double min { get; set; }
        public double max { get; set; }
        public double size { get { return max - min; } }

        public double map(double d)
        {
            return (d - min) / size;
        }

        public double unmap(double d)
        {
            return (d * size) + min;
        }

        public static Range Identity = new Range(0, 1);

        public Range expand(double amt)
        {
            return new Range(min - amt, max + amt);
        }
    };

    /*public class Nest
    {
        IList<Term> terms;
        public IList<Term> Terms { get { return terms; } }

        Level[] levels;
        public Level[] Levels { get { return levels; } }

        Variable variable;
        public Variable Variable { get { return variable; } }

        int height;
        public int Height { get { return height; } }

        public Nest(IList<Term> terms, Level[] levels, int height)
        {
            this.terms = terms;
            this.levels = levels;
            this.height = height;
            this.variable = Constant.Instance;
        }

        public Nest(IList<Term> terms, Level[] levels, Variable v, int height)
        {
            this.terms = terms;
            this.levels = levels;
            this.height = height;
            this.variable = v;
        }

        public double Map(double d)
        {
            d = Variable.map(d);
            foreach (Level v in Levels)
            {
                d = v.map(d);
            }

            return d;
        }

        public double MapConstant(double d)
        {
            foreach (Level v in Levels)
            {
                d = v.map(d);
            }

            return d;
        }

        public double Unmap(double d)
        {
            foreach (Level v in Levels.Reverse())
            {
                d = v.unmap(d);
            }
            d = Variable.unmap(d);
            return d;
        }
    }*/

}