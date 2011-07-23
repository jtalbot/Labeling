using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public class Factor : Vector
    {
        List<int> vector;

        public override int Length
        {
            get { return vector.Count(); }
        }

        public override string Type { get { return "Factor"; } }

        bool ordered;
        public bool Ordered { get { return ordered; } }

        public override Range Range
        {
            get
            {
                return new Range(-0.5, Levels-0.5);
            }           
        }

        protected IList<Level> levels = new List<Level>();
        public Level this [int index] 
        {
            get { return levels[index]; }
        }

        public IList<Level> AllLevels { get { return levels; } }

        public int Levels { get { return levels.Count(); } }

        public Factor(List<int> vector, bool ordered, IList<string> levels)
        {
            this.vector = vector;
            this.ordered = ordered;
            for (int i = 0; i < levels.Count(); i++)
            {
                this.levels.Add(new Level(this, i, levels[i]));
            }
        }

        public override List<T> Select<T>()
        {
            return vector.Select(v => (T)Convert.ChangeType(v, typeof(T))).ToList();
        }

        public class Level
        {
            Factor factor;
            public Factor Factor { get { return factor; } }

            int level;
            string name;

            public int LevelIndex { get { return level; } }

            public Level(Factor factor, int level, string name)
            {
                this.factor = factor;
                this.level = level;
                this.name = name;
            }

            public override string ToString()
            {
                return name;
            }
           
            public Level next()
            {
                if (level + 1 < Factor.Levels)
                    return Factor[level + 1];
                else
                    return null;
            }

            public bool last()
            {
                return level >= Factor.Levels - 1;
            }
        }
    }
}
