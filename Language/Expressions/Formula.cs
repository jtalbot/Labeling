using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public class EmptyFrame : Expression
    {
        public override IEnumerable<Value> Eval(IEnumerable<Value> args)
        {
            yield return new Frame();
        }
    }

    public class Formula : Expression
    {
        Expressions lhs;
        List<Expressions> rhs;

        Expression labels;

        public Expressions Response { get { return lhs; } }
        public List<Expressions> Predictors { get { return rhs; } }
        public Expression Labels { get { return labels; } }

        public Formula()
        {
            lhs = new Expressions();
            lhs.All.Add(Symbol.Constant);
            rhs = new List<Expressions>();
            rhs.Add(new Expressions());
            rhs[0].All.Add(Symbol.Constant);
            labels = new EmptyFrame();
        }

        public Formula(Expression predictor, Expression response)
        {
            lhs = new Expressions();
            lhs.All.Add(predictor);
            rhs = new List<Expressions>();
            rhs.Add(new Expressions());
            rhs[0].All.Add(response);
            labels = new EmptyFrame();
        }

        public override IEnumerable<Value> Eval(IEnumerable<Value> args)
        {
            foreach (Value v in lhs.Eval(args))
            {
                if(!(v is Frame))
                    throw new InvalidOperationException("Formula must get frames");
                
                foreach (Frame r in Cross(args, 0))
                {
                    yield return new ModelFrame(v as Frame, r, labels.Eval(args) as Frame);
                }                
            }
        }

        public IEnumerable<Frame> Cross(IEnumerable<Value> args, int i)
        {
            if (i < rhs.Count())
            {
                foreach (Value v in rhs[i].Eval(args))
                {
                    if(!(v is Frame))
                        throw new InvalidOperationException("Formula must get frames");

                    foreach (Frame w in Cross(args, i + 1))
                    {
                        yield return Frame.Cbind(v as Frame, w);
                    }
                }
            }
            else
            {
                yield return new Frame();
            }
        }

        public override string ToString()
        {
            return "Formula";
        }

    }
}
