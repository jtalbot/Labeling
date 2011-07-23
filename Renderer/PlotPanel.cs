using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Language;

namespace Renderer
{
    public class PlotPanel : Panel
    {
        public Panel Plot { get; protected set; }
        public Panel Left { get; protected set; }
        public Panel Right { get; protected set; }
        public Panel Top { get; protected set; }
        public Panel Bottom { get; protected set; }
        public Panel Overlay { get; protected set; }

        Rectangle screen;

        public RenderState state;
        
        public List<SelectionManager> selectionManagers = new List<SelectionManager>();

        public PlotPanel(RenderState state)
        {
            this.state = state;

            Plot = new Panel();
            Left = new Panel();
            Right = new Panel();
            Top = new Panel();
            Bottom = new Panel();
            Overlay = new Panel();

            Add(Plot);
            Add(Left);
            Add(Right);
            Add(Top);
            Add(Bottom);
            Add(Overlay);

            Plot.SetStroke(Color.Gray, 0.5).SetOrder(100000);

            this.x0 = () => screen.Left;
            this.y0 = () => screen.Top;
            this.x1 = () => screen.Right - 1;
            this.y1 = () => screen.Bottom - 1;

            // flip y axis and put 0 at left
            this.Transform = new Transform
            {
                transform = p => new PointF(p.X - (float)this.x0(), (float)this.y1() - p.Y),
                untransform = p => new PointF(p.X + (float)this.x0(), (float)this.y1() + p.Y)
            };

            Overlay.x0 = () => 0;
            Overlay.y0 = () => 0;
            Overlay.x1 = () => this.Width();
            Overlay.y1 = () => this.Height();

            Overlay.Transform = new Transform
            {
                transform = p => new PointF((float)(p.X * Plot.Width()), (float)(p.Y * Plot.Height())),
                untransform = p => new PointF((float)(p.X / Plot.Width()), (float)(p.Y / Plot.Height()))
            };

            Plot.x0 = () => state.MarginLeft;
            Plot.y0 = () => state.MarginBottom;
            Plot.x1 = () => this.Width() - state.MarginRight;
            Plot.y1 = () => this.Height() - state.MarginTop;

            Plot.Clip = true;

            Plot.Transform = Transform.Range(state.XVisibleRange, state.YVisibleRange).Concat(new Transform
            {
                transform = p => new PointF((float)(p.X * Plot.Width()), (float)(p.Y * Plot.Height())),
                untransform = p => new PointF((float)(p.X / Plot.Width()), (float)(p.Y / Plot.Height()))
            });

            Left.x0 = () => 0;
            Left.y0 = () => state.MarginBottom;
            Left.x1 = () => state.MarginLeft;
            Left.y1 = () => this.Height() - state.MarginTop;

            Left.Transform = Transform.Range(() => Range.Identity, state.YVisibleRange).Concat(new Transform
            {
                transform = p => new PointF((float)(p.X + Left.x1()), (float)(p.Y * Left.Height())),
                untransform = p => new PointF((float)(p.X - Left.x1()), (float)(p.Y / Left.Height()))
            });

            Bottom.x0 = () => state.MarginLeft;
            Bottom.y0 = () => 0;
            Bottom.x1 = () => this.Width() - state.MarginRight;
            Bottom.y1 = () => state.MarginBottom;

            Bottom.Transform = Transform.Range(state.XVisibleRange, () => Range.Identity).Concat(new Transform
            {
                transform = p => new PointF((float)(p.X * Bottom.Width()), (float)(p.Y + Bottom.y1())),
                untransform = p => new PointF((float)(p.X / Bottom.Width()), (float)(p.Y - Bottom.y1()))
            });

            //Right.Transform.Concat(Transform.Range(() =>Range.Identity, env.YVisibleRange));
            //Top.Transform.Concat(Transform.Range(env.XVisibleRange, () => Range.Identity));
        }

        public void render(Graphics g, Rectangle screen)
        {
            this.screen = screen;
            this.render(g, new Transform());
        }

        public IEnumerable<Mark> hit(Point p, Rectangle screen)
        {
            this.screen = screen;
            return this.hit(p, new Transform());
        }

        public IEnumerable<Mark> hit(Rectangle p, Rectangle screen)
        {
            this.screen = screen;
            return this.hit(p, new Transform());
        }

        public void setScreen(Rectangle screen)
        {
            this.screen = screen;
        }
    }
}
