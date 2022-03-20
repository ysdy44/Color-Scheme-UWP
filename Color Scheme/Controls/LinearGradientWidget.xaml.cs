using HSVColorPickers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Color_Scheme.Controls
{
    public sealed partial class LinearGradientWidget : UserControl, IColorPicker
    {
        //@Override
        public string Type => " LinearGradient";
        public Control Self => this;
        public Color Color { get => this.Brush.Color; set => this.Brush.Color = value; }

        public event ColorChangedHandler ColorChanged;
        public event ColorChangedHandler ColorChangedStarted;
        public event ColorChangedHandler ColorChangedDelta;
        public event ColorChangedHandler ColorChangedCompleted;

        Vector2 Scale2;
        Vector2 Vector;
        CanvasRenderTarget RenderTarget;
        int W;
        int H;

        public LinearGradientWidget()
        {
            this.InitializeComponent();

            this.LeftTopButton.Click += (s, e) =>
            {
                this.LeftTopBrush.Color = this.Color;
                using (CanvasDrawingSession ds = this.RenderTarget.CreateDrawingSession())
                {
                    ds.DrawGradientMesh(this.Create(this.CanvasControl));
                }
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.RightTopButton.Click += (s, e) =>
            {
                this.RightTopBrush.Color = this.Color;
                using (CanvasDrawingSession ds = this.RenderTarget.CreateDrawingSession())
                {
                    ds.DrawGradientMesh(this.Create(this.CanvasControl));
                }
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.RightBottomButton.Click += (s, e) =>
            {
                this.RightBottomBrush.Color = this.Color;
                using (CanvasDrawingSession ds = this.RenderTarget.CreateDrawingSession())
                {
                    ds.DrawGradientMesh(this.Create(this.CanvasControl));
                }
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.LeftBottomButton.Click += (s, e) =>
            {
                this.LeftBottomBrush.Color = this.Color;
                using (CanvasDrawingSession ds = this.RenderTarget.CreateDrawingSession())
                {
                    ds.DrawGradientMesh(this.Create(this.CanvasControl));
                }
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // CanvasControl
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.Scale2 = new Vector2((float)e.NewSize.Width / 150, 1);
            };
            this.CanvasControl.CreateResources += (s, args) =>
            {
                this.RenderTarget = new CanvasRenderTarget(s, 150, 150);
                this.W = (int)this.RenderTarget.SizeInPixels.Width;
                this.H = (int)this.RenderTarget.SizeInPixels.Height;
                using (CanvasDrawingSession ds = this.RenderTarget.CreateDrawingSession())
                {
                    ds.DrawGradientMesh(this.Create(s));
                }
            };
            this.CanvasControl.Draw += (s, args) =>
            {
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Scale = this.Scale2,
                    Source = this.RenderTarget
                });
            };


            //Pointer
            this.CanvasControl.PointerPressed += (s, e) =>
            {
                base.CapturePointer(e.Pointer);
            };
            this.CanvasControl.PointerReleased += (s, e) =>
            {
                base.ReleasePointerCapture(e.Pointer);
            };


            //Manipulation
            this.CanvasControl.ManipulationMode = ManipulationModes.All;
            this.CanvasControl.ManipulationStarted += (sender, e) =>
            {
                this.Vector = e.Position.ToVector2();
                this.UpdateEllipse(this.Vector);
                this.UpdateColor(this.Vector);
                this.ColorChangedStarted?.Invoke(this, this.Color); // Delegate
            };
            this.CanvasControl.ManipulationDelta += (sender, e) =>
            {
                this.Vector += e.Delta.Translation.ToVector2();
                this.UpdateEllipse(this.Vector);
                this.UpdateColor(this.Vector);
                this.ColorChangedDelta?.Invoke(this, this.Color); // Delegate
            };
            this.CanvasControl.ManipulationCompleted += (sender, e) =>
            {
                this.UpdateEllipse(this.Vector);
                this.UpdateColor(this.Vector);
                this.ColorChangedCompleted?.Invoke(this, this.Color); // Delegate
            };
        }

        private void UpdateEllipse(Vector2 point)
        {
            double x = Math.Max(0, Math.Min(this.CanvasControl.ActualWidth, point.X));
            double y = Math.Max(0, Math.Min(this.CanvasControl.ActualHeight, point.Y));

            Canvas.SetLeft(this.Ellipse, x - 12);
            Canvas.SetTop(this.Ellipse, y - 12);
        }

        private void UpdateColor(Vector2 point)
        {
            int x = (int)(point.X / (float)this.CanvasControl.ActualWidth * this.W);
            int y = (int)(point.Y / 150 * this.H);

            int left = Math.Max(0, Math.Min(this.W - 1, x));
            int top = Math.Max(0, Math.Min(this.H - 1, y));

            this.Color = this.RenderTarget.GetPixelColors(left, top, 1, 1).Single();
        }

        private CanvasGradientMesh Create(CanvasControl sender) => new CanvasGradientMesh(sender, new CanvasGradientMeshPatch[]
        {
            LinearGradientWidget.CreateCoonsPatch
            (
                150,
                150,
                this.LeftTopBrush.Color,
                this.RightTopBrush.Color,
                this.RightBottomBrush.Color,
                this.LeftBottomBrush.Color
            )
        });

        private static CanvasGradientMeshPatch CreateCoonsPatch(float width, float height, Color color00, Color color10, Color color11, Color color01)
        {
            float x0 = 0;
            float x1 = width / 3;
            float x2 = x1 + x1;
            float x3 = width;

            float y0 = 0;
            float y1 = height / 3;
            float y2 = y1 + y1;
            float y3 = height;

            return new CanvasGradientMeshPatch
            {
                Point01 = new Vector2(x0, y1),
                Point00 = new Vector2(x0, y0),
                Point11 = new Vector2(x1, y1),
                Point10 = new Vector2(x1, y0),

                Point30 = new Vector2(x3, y0),
                Point20 = new Vector2(x2, y0),
                Point31 = new Vector2(x3, y1),
                Point21 = new Vector2(x2, y1),

                Point02 = new Vector2(x0, y2),
                Point03 = new Vector2(x0, y3),
                Point13 = new Vector2(x1, y3),
                Point12 = new Vector2(x1, y2),

                Point32 = new Vector2(x3, y2),
                Point33 = new Vector2(x3, y3),
                Point23 = new Vector2(x2, y3),
                Point22 = new Vector2(x2, y2),

                Color00 = new Vector4(color00.R / 255f, color00.G / 255f, color00.B / 255f, color00.A / 255f),
                Color30 = new Vector4(color10.R / 255f, color10.G / 255f, color10.B / 255f, color10.A / 255f),
                Color33 = new Vector4(color11.R / 255f, color11.G / 255f, color11.B / 255f, color11.A / 255f),
                Color03 = new Vector4(color01.R / 255f, color01.G / 255f, color01.B / 255f, color01.A / 255f),

                Edge00To03 = CanvasGradientMeshPatchEdge.Aliased,
                Edge03To33 = CanvasGradientMeshPatchEdge.Aliased,
                Edge30To00 = CanvasGradientMeshPatchEdge.Aliased,
                Edge33To30 = CanvasGradientMeshPatchEdge.Aliased,
            };
        }

    }
}