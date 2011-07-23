using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Language;
using Layout.Formatters;

namespace Layout
{
    public class Axis
    {
        public enum Direction { HORIZONTAL, VERTICAL };
        
        public Value symbol;
        public Direction direction;

        // Formatting results
        public Range visibleRange;

        public int fontSize;
        public double tickSize;
        public Direction labelDirection;

        public List<Tuple<decimal, string>> labels; //tick placement, label text
        public string axisTitleExtension = "";

        // Statistics and scoring
        public double score;
        
        public double simplicity;
        public double coverage;
        public double granularity;
        public double legibility;

        //testing purposes
        public Format formatStyle;

        public Axis()
        {
            labels = new List<Tuple<decimal, string>>();
            direction = Direction.HORIZONTAL;
            score = -10000000;
            tickSize = 7;
            fontSize = 12;
            visibleRange = new Range(0, 0);
            labelDirection = Direction.HORIZONTAL;


            simplicity = -100000000;
            coverage = -1000000000;
            granularity = -100000000;
            legibility = -10000000;
            formatStyle = null;
        }

        public Axis Clone()
        {
            Axis clone = new Axis();
            clone.symbol = symbol;
            clone.fontSize = fontSize;
            clone.direction = direction;
            clone.labels = new List<Tuple<decimal, string>>(labels);

            clone.score = this.score;
            clone.tickSize = tickSize;
            clone.visibleRange = visibleRange;
            clone.labelDirection = labelDirection;

            clone.simplicity = simplicity;
            clone.coverage = coverage;
            clone.granularity = granularity;
            clone.legibility = legibility;

            clone.formatStyle = formatStyle;

            return clone;
        }

    }
}
