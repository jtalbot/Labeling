using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;

namespace Renderer
{
    /*
    * For each plot we construct an evironment that contains:
    *  The data set from which to draw variables (defined by the Factor.Levels)
    *  The interaction data set for this plot 
    *  The formula for creating a model, including any necessary typing information
    *  The model is created from data set or interaction variables
    *  The plot definition which uses variables from the data set, the interaction, or the model
    */
    public class RenderState
    {
        public ModelFrame Frame;
        
        public double MarginLeft, MarginRight, MarginTop, MarginBottom;
        public double CMarginLeft, CMarginRight, CMarginTop, CMarginBottom;
        
        public Range xVisibleRange = null;
        public Range yVisibleRange = null;

        public Range xDisplayedRange = null;
        public Range yDisplayedRange = null;

        public RenderState()
        {
        }

        public RenderState Clone()
        {
            RenderState result = new RenderState();

            result.Frame = Frame;

            result.xVisibleRange = xVisibleRange;
            result.yVisibleRange = yVisibleRange;
            result.xDisplayedRange = xDisplayedRange;
            result.yDisplayedRange = yDisplayedRange;

            result.MarginLeft = MarginLeft;
            result.MarginRight = MarginRight;
            result.MarginTop = MarginTop;
            result.MarginBottom = MarginBottom;

            result.CMarginLeft = CMarginLeft;
            result.CMarginRight = CMarginRight;
            result.CMarginTop = CMarginTop;
            result.CMarginBottom = CMarginBottom;
            
            return result;
        }

        // This is the desired range
        public Range XRange()
        {
            return xDisplayedRange != null ? xDisplayedRange : Frame.P.Columns[0].Range;
        }

        public Range YRange()
        {
            return yDisplayedRange != null ? yDisplayedRange : Frame.R.Columns[0].Range;
        }
       
        // This is the computed range after the Axes have updated the range.
        public Range YVisibleRange()
        {
            return yVisibleRange != null ? yVisibleRange : Frame.R.Columns[0].Range;
        }

        public Range XVisibleRange()
        {
            return xVisibleRange != null ? xVisibleRange : Frame.P.Columns[0].Range;
        }      
    }
}
