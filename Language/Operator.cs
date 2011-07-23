using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public abstract class Operator
    {
        string name;
        public string Name { get { return name; } }

        Operation operation;
        public Operation Operation { get { return operation; } }

        public Operator(Operation operation, string name)
        {
            this.operation = operation;
            this.name = name;
        }

        public abstract Environment Apply(Environment e);

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            Operator p = obj as Operator;
            if (p == null)
            {
                return false;
            }

            return Operation.Equals(p.Operation) && Name.Equals(p.Name);
        }

        public override int GetHashCode()
        {
            return Operation.GetHashCode() ^ Name.GetHashCode();
        }
    }

    public class FuncOperator : Operator
    {
        Func<Environment, Environment> op;

        public FuncOperator(Operation operation, string name, Func<Environment, Environment> op)
            : base(operation, name)
        {
            this.op = op;
        }

        public override Environment Apply(Environment e)
        {
            return op(e);
        }
    }

    public static class OperatorUtilities
    {
        public static Environment Apply(this IEnumerable<Operator> list, IEnumerable<Symbol> symbols, DataSource data)
        {
            return list.Apply(new Environment(symbols, data));
        }

        public static Environment Apply(this IEnumerable<Operator> list, Environment c)
        {
            foreach (Operator level in list)
            {
                c = level.Apply(c);
                //c.Operators.Add(level);
            }

            return c;
        }
    }
}
