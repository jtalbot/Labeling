using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public interface Value
    {
    }

    public delegate void ChangedHandler(Expression value);
    public delegate void ViewChangedHandler(Expression value);

    // Named values in the language
    public abstract class Expression : Value, System.ComponentModel.INotifyPropertyChanged
    {
        public abstract IEnumerable<Value> Eval(IEnumerable<Value> args);
        public IEnumerable<Value> Eval(params Value[] args)
        {
            return Eval(args.AsEnumerable());
        }

        // Flag changes to value
        public ChangedHandler Changed;

        // Flag changes to visible state (not to actual value)
        public ViewChangedHandler ViewChanged;

        // Hack around fact that Binding List doesn't get the parameter to PropertyChanged.
        public string LastPropertyChange;

        public virtual void NotifyChanged()
        {
            LastPropertyChange = "ValueChanged";

            if (Changed != null)
                Changed(this);

            if (PropertyChanged != null)
                PropertyChanged( this, new System.ComponentModel.PropertyChangedEventArgs("ValueChanged") );
        }

        public virtual void NotifyViewChanged()
        {
            LastPropertyChange = "ViewChanged";

            if (ViewChanged != null)
                ViewChanged(this);

            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("ViewChanged"));
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;


        //
        // Visible properties of the variable
        //
        
        // Is it highlighted?
        protected bool highlighted = true;
        virtual public bool Highlighted { get { return highlighted; } set { if (highlighted != value) { highlighted = value; NotifyViewChanged(); } } }

        // Can be moved around
        protected bool moveable = true;
        virtual public bool Moveable { get { return moveable; } set { if (moveable != value) { moveable = value; NotifyViewChanged(); } } }
    }

    // List of expressions
    public class Expressions : Expression
    {
        bool compared = true;
        public bool Compared { get { return compared; } set { compared = value; NotifyChanged(); } }

        List<Expression> expressions = new List<Expression>();
        public List<Expression> All { get { return expressions; } }

        public override IEnumerable<Value> Eval(IEnumerable<Value> args)
        {
            if (compared)
            {
                foreach (Expression e in expressions)
                    foreach (Value v in e.Eval(args))
                        yield return v;
            }
            else
            {
                if (expressions.Count() > 0)
                    foreach (Value v in expressions[0].Eval(args))
                        yield return v;
            }
        }
    }



}
