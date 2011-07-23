using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;
using System.Diagnostics;
using System.Drawing;

namespace Layout.Formatters
{
    class QuantitativeFormatter : Formatter
    {
        static int[] fontSizes = { 7, 8, 9, 10, 12, 14, 18, 20, 24 };      // Latex default font sizes
        Dictionary<int, float> ems;
                            // In the paper we had a minimum font size of 5, but that's pretty stinking tiny. 7 is probably a better minimum size.

        public QuantitativeFormatter()
        {
            ems = (from x in fontSizes select new { x, size=new Font("Verdana", x).GetHeight(dummyG) }).ToDictionary(a=>a.x, a=>a.size);
        }

        public override Axis format(List<Axis> list, List<Format> formats, AxisLabeler.Options options, Func<Axis, double> ScoreAxis, double bestScore = double.NegativeInfinity)
        {
            Axis result = options.DefaultAxis();
            foreach (Axis data in list)
            {
                foreach (Format format in formats)
                {
                    Axis f = data.Clone();
                    f.formatStyle = format;
                    f.legibility = legibilityScoreMax(f, options);

                    if (ScoreAxis(f) >= bestScore)
                    {
                        Tuple<IEnumerable<string>, string> labels = f.formatStyle.FormalLabels(f.labels.Select(x => (object)(decimal)x.Item1));
                        f.labels = f.labels.Select(x => x.Item1).Zip(labels.Item1, (a, b) => new Tuple<decimal, string>(a, b)).ToList();
                        f.axisTitleExtension = labels.Item2;
                        f.legibility = legibilityScore(f, options);
                        f.score = ScoreAxis(f);
                        if (f.score >= bestScore)
                        {
                            bestScore = f.score;
                            result = f;
                        }
                    }
                }
            }
            return result;
        }

        protected double legibility_format(Axis data, AxisLabeler.Options options)
        {
            double format = data.formatStyle.Score(data.labels.Select(x => (object)x.Item1));
            return format;
        }

        protected double legibility_fontSize(Axis data, AxisLabeler.Options options)
        {
            double fsmin = fontSizes.Min();
            return (data.fontSize > options.fontSize || data.fontSize < fsmin) ? double.NegativeInfinity :
                        ((data.fontSize == options.fontSize) ? 1 :
                            0.2 * ((double)(data.fontSize - fsmin + 1) / (options.fontSize - fsmin)));

        }

        protected double legibility_orientation(Axis data, AxisLabeler.Options options)
        {
            return data.labelDirection == Axis.Direction.HORIZONTAL ? 1.0 : -0.5;
        }

        protected double legibility_overlap(Axis data, AxisLabeler.Options options)
        {
            // compute overlap score
            double em = ems[data.fontSize];
            List<RectangleF> rects = data.labels.Select(s => options.ComputeLabelRect(s.Item2, s.Item1, data)).ToList();
            // takes adjacent pairs of rectangles
            double overlap = rects.Take(rects.Count() - 1).Zip(rects.Skip(1), 
                (a, b) =>
                {
                    double dist = (options.direction == Axis.Direction.HORIZONTAL) ? b.Left - a.Right : a.Top - b.Bottom;
                    return Math.Min(1, 2 - (1.5 * em) / Math.Max(0, dist));
                } ).Min();
            return overlap;
        }

        protected double legibilityScoreMax(Axis data, AxisLabeler.Options options)
        {
            return (legibility_format(data, options) +
                    legibility_fontSize(data, options) +
                    legibility_orientation(data, options) +
                    1) / 4;
        }

        protected double legibilityScore(Axis data, AxisLabeler.Options options)
        {
            return (legibility_format(data, options) +
                    legibility_fontSize(data, options) +
                    legibility_orientation(data, options) +
                    legibility_overlap(data, options)) / 4;
        }

        public List<Axis> varyFontSize(List<Axis> list, AxisLabeler.Options options)
        {
            List<Axis> possibilities = new List<Axis>();
            // Reverse to produce the font sizes in decreasing order of goodness
            foreach (int size in fontSizes.Where(s => s <= options.fontSize).Reverse())
            {
                foreach (Axis data in list)
                {
                    Axis option = data.Clone();
                    option.fontSize = size;
                    possibilities.Add(option);
                }
            }
            return possibilities;
        }

        public List<Axis> varyOrientation(List<Axis> list)
        {
            List<Axis> possibilities = new List<Axis>();
            foreach (Axis data in list)
            {
                Axis option = data.Clone();
                option.labelDirection = Axis.Direction.HORIZONTAL;
                possibilities.Add(option);
            }

            foreach (Axis data in list)
            {
                Axis option = data.Clone();
                option.labelDirection = Axis.Direction.VERTICAL;
                possibilities.Add(option);
            }
            return possibilities;
        }
    }

}
