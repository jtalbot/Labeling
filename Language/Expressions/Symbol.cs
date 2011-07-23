using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public class Symbol : Expression
    {
        protected string name;
        public string Name { get { return name; } set { name = value; NotifyChanged(); } }

        public Symbol(string name)
        {
            this.name = name;
        }

        public override IEnumerable<Value> Eval(IEnumerable<Value> args)
        {
            foreach (Value v in args)
            {
                if (v is Frame)
                {
                    yield return (v as Frame).Project(this);
                }
                else
                    throw new InvalidOperationException("Attempt to apply a symbol to a non-data source");
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Symbol && name == (obj as Symbol).name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override string ToString()
        {
            return name;
        }

        public static readonly Symbol Constant = new Symbol("Constant");
    }
}
