using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public delegate void DataSourceChangedHandler();
    
    public abstract class DataSource : Value
    {
        public abstract IEnumerable<Value> Symbols { get; }
        public abstract int Count { get; }

        public abstract string Formula { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is DataSource))
                return false;

            DataSource ds = obj as DataSource;

            return Formula.Equals(ds.Formula);
        }

        public override int GetHashCode()
        {
            return Formula.GetHashCode();
        }

        //public abstract DataSource Evaluate(string formula);
        //public abstract List<T> Apply<T>(string formula, int dimension);
        
        public abstract DataSource Where(IEnumerable<string> conditions);
        public DataSource Where(IEnumerable<Factor.Level> conditions)
        {
            return Where(conditions.Select(c => c.Clause()));
        }
        public DataSource Where(params string[] conditions)
        {
            return Where(conditions.AsEnumerable());
        }
        public DataSource Where(params Factor.Level[] conditions)
        {
            return Where(conditions.AsEnumerable());
        }

        // Whole bunch of Select variations. All others go through the first two here, so they should be overriden by descendents.
        public abstract T SelectOne<T>(string formula);        
        public abstract List<T> Select<T>(string formula);
        

        public List<List<T>> Select<T>(IEnumerable<string> formulas)
        {
            List<List<T>> result = new List<List<T>>(formulas.Count());
            foreach (string formula in formulas)
            {
                result.Add(Select<T>(formula));
            }
            return result;
        }

        public List<List<T>> Select<T>(params string[] formulas)
        {
            return Select<T>(formulas.AsEnumerable());
        }

        public List<List<T>> Select<T>(IEnumerable<Language.Value> terms)
        {
            // TODO: move this Factor hack elsewhere...
            return Select<T>(from t in terms
                             select
                                t.Formula);
                                 //(t is Factor) ? "unclass(" + t.Formula + ")-1" : t.Formula);
        }

        public List<List<T>> Select<T>(params Language.Value[] terms)
        {
            return Select<T>((IEnumerable<Language.Value>)terms);
        }

        public List<T> Select<T>(Language.Value term)
        {
            // TODO: move this Factor hack elsewhere...
            return Select<T>((term is Factor) ? "unclass(" + term.Formula + ")-1" : term.Formula);
        }

        // Untyped queries        
        public List<List<object>> Select(IEnumerable<string> formulas)
        {
            return Select<object>(formulas);
        }

        public List<List<object>> Select(params string[] formulas)
        {
            return Select<object>(formulas);
        }

        public List<object> Select(string formula)
        {
            return Select<object>(formula);
        }

        public object SelectOne(string formula)
        {
            return SelectOne<object>(formula);
        }

        public List<List<object>> Select(IEnumerable<Language.Value> terms)
        {
            return Select<object>(terms);
        }

        public List<List<object>> Select(params Language.Value[] terms)
        {
            return Select<object>((IEnumerable<Language.Value>)terms);
        }

        public List<object> Select(Language.Value term)
        {
            return Select<object>(term);
        }
    }
}
