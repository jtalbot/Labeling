using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Layout.Formatters
{
    abstract class Formatter
    {
        protected Graphics dummyG = Graphics.FromImage(new Bitmap(1, 1));
        public abstract Axis format(List<Axis> list, List<Format> formats, AxisLabeler.Options options, Func<Axis, double> ScoreAxis, double bestScore = double.NegativeInfinity);
    }

}
