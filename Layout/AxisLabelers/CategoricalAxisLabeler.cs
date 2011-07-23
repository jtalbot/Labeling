using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;

namespace Layout
{
    // Categorical labeling code
    class CategoricalAxisLabeler : AxisLabeler
    {
        public override Axis generate(Options options, double density)
        {
            Factor f = options.symbol as Factor;

            List<Tuple<decimal, string>> labels = new List<Tuple<decimal, string>>();

            for (int j = 0; j < f.AllLevels.Count(); j++)
            {
                labels.Add(new Tuple<decimal, string>(f.AllLevels[j].LevelIndex, f.AllLevels[j].ToString()));
            }

            Axis result = options.DefaultAxis();
            result.labels = labels;

            return result;
        }
    }
}
