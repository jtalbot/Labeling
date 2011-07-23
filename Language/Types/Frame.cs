using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public abstract class Vector : Value
    {
        public abstract Range Range { get; }
        public abstract string Type { get; }
        public abstract int Length { get; }

        public abstract List<T> Select<T>();
    }

    public class Frame : Value
    {
        public List<Vector> Columns = new List<Vector>();
        public List<Symbol> Symbols = new List<Symbol>();

        public int NumColumns { get { return Columns.Count(); } }
        int numRows;
        public int NumRows { get { return numRows; } }

        public Frame() 
        {
            numRows = 0;
        }

        public Frame(Vector v, Symbol s)
        {
            Columns.Add(v);
            Symbols.Add(s);
            numRows = v.Length;
        }

        public static Frame Cbind(Frame a, Frame b)
        {
            if (!(a.NumColumns == 0 || b.NumColumns == 0 || a.NumRows == b.NumRows))
                throw new InvalidOperationException("Cbind only works on frames with the same number of rows");
            Frame result = new Frame();
            result.Columns = a.Columns.Concat(b.Columns).ToList();
            result.Symbols = a.Symbols.Concat(b.Symbols).ToList();
            result.numRows = Math.Max(a.numRows, b.NumRows);
            return result;
        }

        public Frame Project(IEnumerable<Symbol> symbols)
        {
            Frame result = new Frame();
            foreach (Symbol s in symbols)
            {
                int i = Symbols.IndexOf(s);
                if (i < 0)
                    throw new InvalidOperationException("Tried to project non-existent column " + s.Name);
                result.Columns.Add(Columns[i]);
                result.Symbols.Add(Symbols[i]);
            }
            result.numRows = numRows;
            return result;
        }

        public Frame Project(params Symbol[] symbols)
        {
            return Project(symbols.AsEnumerable());
        }

        public List<T> Select<T>(int i)
        {
            return Columns[i].Select<T>();
        }
    }

    public class ModelFrame : Value
    {
        public Frame R, P, L;

        public ModelFrame(Frame r, Frame p, Frame l)
        {
            R = r;
            P = p;
            L = l;
        }
    }

    /*
    public class Frame : Expression
    {
        public IEnumerable<Value> Eval(IEnumerable<Value> args)
        {
            yield return this;
        }

        // Symbol name (public facing)
        protected string name;
        public string Name { get { return name; } set { name = value; NotifyChanged(); } }

        public IEnumerable<Symbol> Symbols;
        public DataSource Source;

        public Frame(DataSource source, params Symbol[] symbols)
        {
            name = "";
            Symbols = symbols.AsEnumerable();
            Source = source;
        }

        public Frame(DataSource source, IEnumerable<Symbol> symbols)
        {
            name = "";
            Symbols = symbols;
            Source = source;
        }
    }
     */
}
