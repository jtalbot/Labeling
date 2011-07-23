using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Language;
using System.Windows.Forms;

namespace Renderer
{
    public class Transform
    {
        public Func<PointF, PointF> transform = (p => p);
        public Func<PointF, PointF> untransform = (p => p);

        public Transform Concat(Transform t)
        {
            return new Transform
            {
                transform = point => t.transform(this.transform(point)),
                untransform = point => this.untransform(t.untransform(point))
            };
        }

        public static Transform Translate(PointF p)
        {
            return new Transform
            {
                transform = point => new PointF(point.X + p.X, point.Y+p.Y),
                untransform = point => new PointF(point.X - p.X, point.Y - p.Y)
            };
        }

        public static Transform Range(Range x, Range y)
        {
            return new Transform
            {
                transform = point => new PointF((float)x.map(point.X), (float)y.map(point.Y)),
                untransform = point => new PointF((float)x.unmap(point.X), (float)y.unmap(point.Y)),
            };
        }

        public static Transform Range(Func<Range> x, Func<Range> y)
        {
            return new Transform
            {
                transform = point => new PointF((float)x().map(point.X), (float)y().map(point.Y)),
                untransform = point => new PointF((float)x().unmap(point.X), (float)y().unmap(point.Y)),
            };
        }
    }

    public abstract class SelectionManager
    {
        public abstract bool InteractiveUpdate(SelectionMode mode, IEnumerable<Mark> marks);
        public abstract bool Done(bool commit);
    }

    public abstract class Mark
    {
        public Func<Pen> fStroke;
        public Pen Stroke { get { return fStroke(); } set { fStroke = () => value; } }

        public String Name { get; set; }
        public SelectionManager SelectionManager { get; set; }
        public Brush Fill { get; set; }
        public object Data { get; set; }
        public double Order { get; set; }
        public Panel Parent { get; set; }
        public bool Visible { get; set; }        

        public Func<PointF, bool> MouseOver, MouseEnter, MouseExit;        

        public Mark()
        {
            Name = null;
            SelectionManager = null;
            Stroke = new Pen(Color.Transparent, 1);
            Fill = new SolidBrush(Color.Transparent);            
            Order = 0;
            MouseOver = p => false;
            MouseEnter = p => false;
            MouseExit = p => false;
            Visible = true;
        }

        public abstract void render(Graphics g, Transform transform);
        public abstract bool hitTest(Point p, Transform transform);
        public abstract bool hitTest(Rectangle hit, Transform transform);
        public virtual RectangleF bounds(Transform transform)
        {
            throw new NotImplementedException();
        }


        // Interaction state
        public bool hovered = false;
    }

    public class BarMark : Mark
    {
        public Func<double> x0, y0, x1, y1;
        public Func<double> Width, Height;

        public BarMark()
            : base()
        {
            x0 = () => 0;
            y0 = () => 0;
            x1 = () => 1;
            y1 = () => 1;

            Width = () => x1() - x0();
            Height = () => y1() - y0();
        }

        public BarMark(double x0, double y0, double x1, double y1)
        {
            this.x0 = () => x0;
            this.y0 = () => y0;
            this.x1 = () => x1;
            this.y1 = () => y1;

            Width = () => this.x1() - this.x0();
            Height = () => this.y1() - this.y0();
        }

        public override void render(Graphics g, Transform t)
        {
            PointF zero = t.transform(new PointF((float)x0(), (float)y0()));
            PointF one = t.transform(new PointF((float)x1(), (float)y1()));
            RectangleF rect = RendererUtilities.MakeRectangleF(zero.X, zero.Y, one.X, one.Y);

            System.Drawing.Drawing2D.SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.FillRectangle(Fill, rect);
            g.DrawRectangle(Stroke, rect.X, rect.Y, rect.Width, rect.Height);
            g.SmoothingMode = oldMode;
        }

        public override bool hitTest(Point p, Transform t)
        {
            RectangleF rect = bounds(t);
            return rect.Contains(p);
        }

        public override bool hitTest(Rectangle hit, Transform t)
        {
            RectangleF rect = bounds(t);
            return rect.IntersectsWith(hit);
        }

        public override RectangleF bounds(Transform t)
        {
            PointF zero = t.transform(new PointF((float)x0(), (float)y0()));
            PointF one = t.transform(new PointF((float)x1(), (float)y1()));
            RectangleF r = RendererUtilities.MakeRectangleF(zero.X, zero.Y, one.X, one.Y);
            r.Inflate(Stroke.Width / 2.0f, Stroke.Width / 2.0f);
            return r;
            //return Children.OrderBy(m => m.Order).Aggregate(RectangleF.Empty, (t, m) => t.Union(m.bounds(transform.Concat(Transform))));
        }
    }

    public class Panel : BarMark
    {
        public List<Mark> Children { get; set; }
        // transformation of children
        public Transform Transform { get; set; }

        public bool Clip = false;

        public Color color = Color.Transparent;

        public Panel()
            : base()
        {
            Transform = new Transform();
            Children = new List<Mark>();
            this.SetStrokeColor(Color.Transparent);
            this.SetFillColor(Color.Transparent);
        }

        public void Add(Mark mark)
        {
            Children.Add(mark);
            mark.Parent = this;
        }

        public Transform ToScreen()
        {
            if (Parent != null)
                return Transform.Concat(Transform.Translate(new PointF((float)x0(), (float)y0()))).Concat(Parent.ToScreen());
            else
                return Transform;
        }

        public IEnumerable<Mark> hit(Point p, Transform t)
        {
            Transform nt = Transform.Concat(Transform.Translate(new PointF((float)x0(), (float)y0()))).Concat(t);
            foreach (Mark c in Children.OrderBy(m => m.Order))
            {
                if (c.hitTest(p, nt))
                    yield return c;

                if (c is Panel)
                {
                    foreach (Mark m in (c as Panel).hit(p, nt))
                        yield return m;
                }
            }
        }

        public IEnumerable<Mark> hit(Rectangle p, Transform t)
        {
            Transform nt = Transform.Concat(Transform.Translate(new PointF((float)x0(), (float)y0()))).Concat(t);
            foreach (Mark c in Children.OrderBy(m => m.Order))
            {
                if (c.hitTest(p, nt))
                    yield return c;

                if (c is Panel)
                {
                    foreach (Mark m in (c as Panel).hit(p, nt))
                        yield return m;
                }
            }
        }  
    }

    public class DotMark : Mark
    {
        double x, y;
        float size = 5;
        public enum Shape { CIRCLE, X };
        Shape shape;

        public DotMark(double x, double y) : base()
        {
            this.x = x;
            this.y = y;
            this.shape = Shape.CIRCLE;
        }

        public DotMark SetSize(double d)
        {
            size = (float)d;
            return this;
        }

        public DotMark SetShape(Shape shape)
        {
            this.shape = shape;
            return this;
        }

        public override void  render(Graphics g, Transform transform)
        {
            if (!Visible)
                return;

            PointF s = transform.transform(new PointF((float)x, (float)y));

            switch(shape)
            {
                case Shape.CIRCLE:
                    g.FillEllipse(Fill, s.X - (size / 2), s.Y - (size / 2), size, size);
                    g.DrawEllipse(Stroke, s.X - (size / 2), s.Y - (size / 2), size, size);
                    break;
                case Shape.X:
                    g.DrawLine(Stroke, (float)(s.X - (size / 2)), (float)(s.Y - (size / 2)), (float)(s.X + (size / 2)), (float)(s.Y + (size / 2)));
                    g.DrawLine(Stroke, (float)(s.X - (size / 2)), (float)(s.Y + (size / 2)), (float)(s.X + (size / 2)), (float)(s.Y - (size / 2)));
                    break;
            }
        }

        public override bool hitTest(Point p, Transform transform)
        {
            PointF s = transform.transform(new PointF((float)x, (float)y));

            float pointsize = (float)(size * 1.3333);
            
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(s.X - (pointsize / 2), s.Y - (pointsize / 2), pointsize, pointsize);
            return gp.IsVisible(p);
        }

        public override bool hitTest(Rectangle hit, Transform transform)
        {
            PointF s = transform.transform(new PointF((float)x, (float)y));

            float pointsize = (float)(size * 1.3333);

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(s.X - (pointsize / 2), s.Y- (pointsize / 2), pointsize, pointsize);
            Region rgn = new Region(gp);
            return rgn.IsVisible(hit);
        }

        public override RectangleF bounds(Transform t)
        {
            PointF zero = t.transform(new PointF((float)x, (float)y));
            RectangleF r = RendererUtilities.MakeRectangleF(zero.X, zero.Y, zero.X, zero.Y);
            r.Inflate(size / 2.0f + Stroke.Width / 2.0f, size / 2.0f + Stroke.Width / 2.0f);
            return r;
            //return Children.OrderBy(m => m.Order).Aggregate(RectangleF.Empty, (t, m) => t.Union(m.bounds(transform.Concat(Transform))));
        }
    }

    public class LineMark : Mark
    {
        List<double> x, y;
        
        public LineMark(IEnumerable<double> x, IEnumerable<double> y) : base()
        {
            this.x = x.ToList();
            this.y = y.ToList();
            this.Stroke.Width = 2;
        }

        public LineMark(double x1, double x2, double y1, double y2) :
            this(new List<double> {x1, x2}, new List<double>{y1, y2})
        {}

        public LineMark SetPosition(double x1, double x2, double y1, double y2)
        {
            x = new List<double> { x1, x2 };
            y = new List<double> { y1, y2 };
            return this;
        }

        public override void  render(Graphics g, Transform t)
        {
            if (!Visible)
                return;

            if(x.Count() < 2)
                return;

            var points = x.Zip(y, (px, py) => t.transform(new PointF((float)px, (float)py))).ToArray();

            System.Drawing.Drawing2D.SmoothingMode oldMode = g.SmoothingMode;

            if(points.Count() == 2 && (points[0].X == points[1].X || points[0].Y == points[1].Y))
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            g.DrawLines(Stroke, points);

            g.SmoothingMode = oldMode;
        }

        public override bool hitTest(Point p, Transform t)
        {
            if(x.Count() < 2)
                return false;

            var points = x.Zip(y, (px, py) => t.transform(new PointF((float)px, (float)py)));

            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(points.ToArray());
            return gp.IsVisible(p) || gp.IsOutlineVisible(p, Stroke);
        }

        public override bool hitTest(Rectangle hit, Transform t)
        {
            if (x.Count() < 2)
                return false;

            var points = x.Zip(y, (px, py) => t.transform(new PointF((float)px, (float)py)));

            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(points.ToArray());
            return new Region(gp).IsVisible(hit);
        }

        public override RectangleF bounds(Transform t)
        {
            var points = x.Zip(y, (px, py) => t.transform(new PointF((float)px, (float)py))).ToArray();
            float x1 = points.Select(p => p.X).Min(), y1 = points.Select(p => p.Y).Min(), x2 = points.Select(p => p.X).Max(), y2 = points.Select(p => p.Y).Max();
            RectangleF r = new RectangleF(x1, y1, x2 - x1, y2 - y1);
            r.Inflate(Stroke.Width / 2.0f, Stroke.Width / 2.0f);
            return r;
        }
    }

    public class AreaMark : Mark
    {
        List<double> x, y;

        public AreaMark(IEnumerable<double> x, IEnumerable<double> y) : base()
        {
            this.x = x.ToList();
            this.y = y.ToList();
            this.Stroke.Color = Color.Transparent;
            this.Fill = new SolidBrush(Color.LightBlue);
        }

        public AreaMark(IEnumerable<double> xb, IEnumerable<double> yb, 
                                IEnumerable<double> xt, IEnumerable<double> yt)
            : this(xb.Concat(xt.Reverse()), yb.Concat(yt.Reverse()))
        {}

        public override void  render(Graphics g, Transform t)
        {
            if (!Visible)
                return;

            if(x.Count() < 3)
                return;

            var points = x.Zip(y, (px, py) => t.transform(new PointF((float)px, (float)py))).ToArray();

            g.FillPolygon(Fill, points);
            //g.DrawPolygon(Stroke, points);
            g.DrawLines(Stroke, points.Take(points.Count() / 2).ToArray());
            g.DrawLines(Stroke, points.Skip(points.Count() / 2).ToArray());
        }

        public override bool  hitTest(Point p, Transform t)
        {
            if(x.Count() < 3)
                return false;

            var points = x.Zip(y, (px, py) => t.transform(new PointF((float)px, (float)py))).ToArray();

            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(points);
            return gp.IsVisible(p);
        }

        public override bool hitTest(Rectangle hit, Transform t)
        {
            if (x.Count() < 3)
                return false;

            var points = x.Zip(y, (px, py) => t.transform(new PointF((float)px, (float)py))).ToArray();

            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(points.ToArray());
            return new Region(gp).IsVisible(hit);
        }

        public override RectangleF bounds(Transform t)
        {
            var points = x.Zip(y, (px, py) => t.transform(new PointF((float)px, (float)py))).ToArray();
            float x1 = points.Select(p => p.X).Min(), y1 = points.Select(p => p.Y).Min(), x2 = points.Select(p => p.X).Max(), y2 = points.Select(p => p.Y).Max();
            RectangleF r = new RectangleF(x1, y1, x2 - x1, y2 - y1);
            r.Inflate(Stroke.Width / 2.0f, Stroke.Width / 2.0f);
            return r;
        }
    }

    public class LabelMark : Mark
    {
        public double x, y;
        string originalText;
        ContentAlignment alignment;
        string fontFamily = "Tahoma";
        float fontSize = 8f;
        SizeF size;
        bool horizontal;

        static Dictionary<Tuple<string, string, float, FontStyle>, SizeF> CachedSizes = new Dictionary<Tuple<string, string, float, FontStyle>, SizeF>();

        enum BlockPlacement { SUPERSCRIPT, SUBSCRIPT };
        struct Block
        {
            public string text;
            public FontStyle style;
            public List<BlockPlacement> placement;
        }
        List<Block> blocks = new List<Block>();

        static public Graphics dummyG = Graphics.FromImage(new Bitmap(1, 1));

        public LabelMark(string text, double x, double y, Color color, ContentAlignment alignment, bool formatted=false) : base()
        {
            this.originalText = text;
            this.x = x;
            this.y = y;
            this.Stroke.Color = color;
            this.Fill = new SolidBrush(Color.Transparent);
            this.alignment = alignment;
            this.horizontal = true;
            if (formatted)
                parseText();
            else
                blocks.Add(new Block { text = text, style = FontStyle.Regular, placement = new List<BlockPlacement>()});
            FontSize(8);
        }

        public LabelMark SetPosition(double x, double y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        public LabelMark SetText(string text, bool formatted=false)
        {
            this.originalText = text;
            blocks.Clear();
            if (formatted)
                parseText();
            else
                blocks.Add(new Block { text = text, style = FontStyle.Regular, placement = new List<BlockPlacement>() });
            this.size = measureString();
            return this;
        }

        public LabelMark FontSize(float size)
        {
            this.fontSize = size;
            this.size = measureString();
            return this;
        }

        private void parseText()
        {
            blocks.Clear();

            List<char> state = new List<char>();
            List<char> text = new List<char>();

            bool useNext = false;
            for (int i = 0; i < originalText.Count(); i++)
            {
                char c = originalText[i];

                switch (c)
                {
                    case '\\':
                        useNext = true;
                        break;
                    case '_':
                    case '*':
                    case '-':
                    case '+':
                    case '^':
                    case '~':
                        if (useNext)
                        {
                            if (text.Count() > 0)
                            {
                                addBlock(new string(text.ToArray()), state);
                            }

                            if (state.Count() > 0 && state.Last() == c)
                            {
                                state.RemoveAt(state.Count() - 1);
                            }
                            else
                            {
                                state.Add(c);
                            }
                            text.Clear();
                        }
                        else
                        {
                            text.Add(c);
                        }
                        useNext = false;
                        break;
                    default:
                        if (useNext)
                        {
                            text.Add('\\');
                        }
                        text.Add(c);
                        useNext = false;
                        break;
                }
            }
            state.Clear();
            if (text.Count() > 0)
                addBlock(new string(text.ToArray()), state);
        }

        private void addBlock(string text, List<char> state)
        {
            FontStyle style = FontStyle.Regular;
            List<BlockPlacement> placement = new List<BlockPlacement>();

            foreach(char c in state)
            {
                switch (c)
                {
                    case '_':
                        style |= FontStyle.Italic;
                        break;
                    case '*':
                        style |= FontStyle.Bold;
                        break;
                    case '-':
                        style |= FontStyle.Strikeout;
                        break;
                    case '+':
                        style |= FontStyle.Underline;
                        break;
                    case '^':
                        placement.Add(BlockPlacement.SUPERSCRIPT);
                        break;
                    case '~':
                        placement.Add(BlockPlacement.SUBSCRIPT);
                        break;
                    default:
                        break;
                }
            }

            blocks.Add(new Block { text = text, style = style, placement = placement });
        }

        private SizeF measureString()
        {
            SizeF result = new SizeF(0, 0);
            foreach (Block b in blocks)
            {
                float fontsize = fontSize;
                foreach (BlockPlacement bp in b.placement)
                {
                    if (bp == BlockPlacement.SUBSCRIPT)
                    {
                        fontsize *= 0.62f;
                    }
                    else
                    {
                        fontsize *= 0.6f;
                    }
                }
                Tuple<string, string, float, FontStyle> tuple = new Tuple<string, string, float, FontStyle>(b.text, fontFamily, fontsize, b.style);
                if (!CachedSizes.ContainsKey(tuple))
                {
                    Font font = new Font(fontFamily, fontsize, b.style);
                    SizeF csize = dummyG.MeasureString(b.text, font, new PointF(0, 0), StringFormat.GenericTypographic);
                    CachedSizes.Add(tuple, csize);
                }
                this.size = CachedSizes[tuple];
                result.Width += this.size.Width;
                result.Height = Math.Max(result.Height, size.Height);
            }
            return result;
        }

        public LabelMark SetHorizontal(bool horizontal)
        {
            this.horizontal = horizontal;
            return this;
        }

        private float offsetX(SizeF size)
        {
            if (horizontal)
            {
                if (alignment == ContentAlignment.BottomLeft ||
                    alignment == ContentAlignment.MiddleLeft ||
                    alignment == ContentAlignment.TopLeft)
                    return 0;
                else if (alignment == ContentAlignment.BottomCenter ||
                    alignment == ContentAlignment.MiddleCenter ||
                    alignment == ContentAlignment.TopCenter)
                    return -size.Width / 2;
                else
                    return -size.Width;
            }
            else
            {
                if (alignment == ContentAlignment.BottomLeft ||
                    alignment == ContentAlignment.MiddleLeft ||
                    alignment == ContentAlignment.TopLeft)
                    return 0;
                else if (alignment == ContentAlignment.BottomCenter ||
                    alignment == ContentAlignment.MiddleCenter ||
                    alignment == ContentAlignment.TopCenter)
                    return -size.Height / 2;
                else
                    return -size.Height;
            }
        }

        private float offsetY(SizeF size)
        {
            if (horizontal)
            {
                if (alignment == ContentAlignment.BottomLeft ||
                    alignment == ContentAlignment.BottomCenter ||
                    alignment == ContentAlignment.BottomRight)
                    return -size.Height;
                else if (alignment == ContentAlignment.MiddleLeft ||
                    alignment == ContentAlignment.MiddleCenter ||
                    alignment == ContentAlignment.MiddleRight)
                    return -size.Height / 2;
                else
                    return 0;
            }
            else
            {
                if (alignment == ContentAlignment.BottomLeft ||
                    alignment == ContentAlignment.BottomCenter ||
                    alignment == ContentAlignment.BottomRight)
                    return 0;
                else if (alignment == ContentAlignment.MiddleLeft ||
                    alignment == ContentAlignment.MiddleCenter ||
                    alignment == ContentAlignment.MiddleRight)
                    return -size.Width / 2;
                else
                    return -size.Width;
            }
        }

        public override void  render(Graphics g, Transform t)
        {
            if (!Visible)
                return;

            if(originalText == "")
                return;

            var s = t.transform(new PointF((float)x, (float)y));

            System.Drawing.Drawing2D.SmoothingMode oldMode = g.SmoothingMode;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            if (!horizontal)
            {
                g.TranslateTransform(s.X, s.Y);
                g.RotateTransform(-90);
                g.FillRectangle(Fill, bounds(t));
                drawBlocks(g, offsetY(size), offsetX(size));
                g.ResetTransform();
            }
            else
            {
                g.FillRectangle(Fill, bounds(t));
                s.X += offsetX(size);
                s.Y += offsetY(size);                
                drawBlocks(g, s.X, s.Y);
            }

            g.SmoothingMode = oldMode;
        }

        public void drawBlocks(Graphics g, float x, float y)
        {            
            Font basicFont = new Font(fontFamily, fontSize);
            float lineHeight = basicFont.GetHeight(g);

            Brush strokeBrush = new SolidBrush(Stroke.Color);

            foreach (Block b in blocks)
            {
                float fontscale = 1;
                float baseline = 0;
                foreach (BlockPlacement bp in b.placement)
                {
                    if (bp == BlockPlacement.SUPERSCRIPT)
                    {
                        baseline = baseline + 0.44f * fontscale;
                        fontscale *= 0.62f;                        
                    }
                    else
                    {
                        baseline = baseline - 0.16f * fontscale;
                        fontscale *= 0.6f;
                    }
                }

                Font font = new Font(fontFamily, fontSize*fontscale, b.style);
                float fontHeight = font.GetHeight(g);
                
                g.DrawString(b.text, font, strokeBrush, x, y+(1-baseline)*lineHeight-fontHeight, StringFormat.GenericTypographic);
                SizeF size = dummyG.MeasureString(b.text, font, new PointF(0, 0), StringFormat.GenericTypographic);
                x += size.Width+0.75f;
            }
        }

        public override bool hitTest(Point p, Transform t)
        {
            if (originalText == "")
                return false;

            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(bounds(t));
            return gp.IsVisible(p);
        }

        public override bool hitTest(Rectangle hit, Transform t)
        {
            if (originalText == "")
                return false;

            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(bounds(t));
            return new Region(gp).IsVisible(hit);
        }

        public override RectangleF bounds(Transform t)
        {
            var s = t.transform(new PointF((float)x, (float)y));
            s.X += offsetX(size);
            s.Y += offsetY(size);
            if(horizontal)
                return new RectangleF(s.X, s.Y, size.Width, size.Height);
            else
                return new RectangleF(s.X, s.Y, size.Height, size.Width);
        }

        public SizeF Size()
        {
            return size;
        }
    }

    public static class RendererUtilities
    {
        public static RectangleF Union(this RectangleF r1, RectangleF r2)
        {
            if (r1.IsEmpty)
                return new RectangleF(r2.X, r2.Y, r2.Width, r2.Height);
            else if (r2.IsEmpty)
                return new RectangleF(r1.X, r1.Y, r1.Width, r1.Height);
            else
            {
                float x = Math.Min(r1.X, r2.X), y = Math.Min(r1.Y, r2.Y);
                return new RectangleF(x, y, Math.Max(r1.X + r1.Width, r2.X + r2.Width) - x, Math.Max(r1.Y + r1.Height, r2.Y + r2.Height) - y);
            }
        }

        public static RectangleF MakeRectangleF(float x1, float y1, float x2, float y2)
        {
            float xm = Math.Min(x1, x2);
            float xM = Math.Max(x1, x2);

            float ym = Math.Min(y1, y2);
            float yM = Math.Max(y1, y2);

            return new RectangleF(xm, ym, xM - xm, yM - ym);
        }
    }

    public static class MarkUtilities
    {
        public static T SetName<T>(this T t, String name) where T : Mark
        {
            t.Name = name;
            return t;
        }

        public static T SetSelectionManager<T>(this T t, SelectionManager manager) where T : Mark
        {
            t.SelectionManager = manager;
            return t;
        }

        public static T SetStroke<T>(this T t, Func<Pen> stroke) where T : Mark
        {
            t.fStroke = stroke;
            return t;
        }

        public static T SetStroke<T>(this T t, Pen stroke) where T : Mark
        {
            t.Stroke = stroke;
            return t;
        }

        public static T SetFill<T>(this T t, Brush fill) where T : Mark
        {
            t.Fill = fill;
            return t;
        }

        public static T SetStroke<T>(this T t, Color stroke, double width) where T : Mark
        {
            t.Stroke.Color = stroke;
            t.Stroke.Width = (float)width;
            return t;
        }

        public static T SetStrokeColor<T>(this T t, Color stroke) where T : Mark
        {
            t.Stroke.Color = stroke;
            return t;
        }

        public static T SetStrokeWidth<T>(this T t, double width) where T : Mark
        {
            t.Stroke.Width = (float)width;
            return t;
        }

        public static T SetFillColor<T>(this T t, Color fill) where T : Mark
        {
            t.Fill = new SolidBrush(fill);
            return t;
        }

        public static T SetData<T>(this T t, object data) where T : Mark
        {
            t.Data = data;
            return t;
        }

        public static T SetOrder<T>(this T t, double order) where T : Mark
        {
            t.Order = order;
            return t;
        }

        public static T SetVisible<T>(this T t, bool v) where T : Mark
        {
            t.Visible = v;
            return t;
        }

        public static T SetMouseOver<T>(this T t, Func<PointF, bool> f) where T : Mark
        {
            t.MouseOver = f;
            return t;
        }

        public static T SetMouseEnter<T>(this T t, Func<PointF, bool> f) where T : Mark
        {
            t.MouseEnter = f;
            return t;
        }

        public static T SetMouseExit<T>(this T t, Func<PointF, bool> f) where T : Mark
        {
            t.MouseExit = f;
            return t;
        }
    }
}
