using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;
using System.Drawing;

namespace Layout
{
    abstract public class AxisLabeler
    {
        // input to the optimization routines.
        public class Options
        {
            public Axis.Direction direction;
            public int fontSize;
            public Vector symbol;
            public Range dataRange;
            public Range visibleRange;
            public RectangleF screen;
            public Func<string, decimal, Axis, RectangleF> ComputeLabelRect;
            
            public Axis DefaultAxis()
            {
                Axis def = new Axis();

                def.fontSize = this.fontSize;
                def.direction = this.direction;
                def.symbol = this.symbol;
                def.visibleRange = this.visibleRange;

                return def;
            }
        }

        public abstract Axis generate(Options options, double m);
    }

    public class NoOpAxisLabeler : AxisLabeler
    {
        public override Axis generate(Options options, double m)
        {
            return options.DefaultAxis();
        }
    }
}
