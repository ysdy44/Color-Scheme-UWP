using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme
{
    public sealed partial class MainPage : Page
    {
        public bool CanWork { get => this.CanvasControl.IsHitTestVisible; set => this.CanvasControl.IsHitTestVisible = value; }
        public UIElement Target => this.CanvasControl;

        Vector2 Vector;
        CanvasBitmap Bitmap;

        private void ConstructCanvasControl()
        {
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.Transformer.Size = e.NewSize;
            };

            this.CanvasControl.Draw += (s, args) =>
            {
                if (this.Bitmap != null) args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    BorderMode = EffectBorderMode.Hard,
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    TransformMatrix = this.Transformer.GetMatrix(),
                    Source = this.Bitmap,
                });

                args.DrawingSession.DrawRuler(this.Transformer,
                    CanvasDrawingSessionExtensions.RulerWidth,
                    CanvasDrawingSessionExtensions.RulerLine,
                    CanvasDrawingSessionExtensions.AxisThickLine,
                    Colors.Transparent, // CanvasDrawingSessionExtensions.RulerBackgroundColor, 
                    CanvasDrawingSessionExtensions.RulerColor,
                    CanvasDrawingSessionExtensions.RulerLineColor,
                    CanvasDrawingSessionExtensions.RulerThickLineColor,
                    CanvasDrawingSessionExtensions.TextColor,
                    CanvasDrawingSessionExtensions.TextFormat);
            };
            this.ThumbnailCanvasControl.Draw += (s, args) =>
            {
                if (this.Bitmap == null) return;

                Vector2 point = this.Vector;
                Matrix3x2 matrix = this.Transformer.GetInverseMatrix();
                Vector2 inversePoint = Vector2.Transform(point, matrix);

                int x = (int)inversePoint.X;
                int y = (int)inversePoint.Y;

                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.Bitmap,
                    TransformMatrix =
                      Matrix3x2.CreateTranslation(-inversePoint)
                      * Matrix3x2.CreateScale(this.Transformer.Scale * 2)
                      * Matrix3x2.CreateTranslation(112 / 2, 90 / 2)
                });
            };
        }

        private void ConstructCanvasOperator()
        {
            // Single
            this.Operator.Single_Start += (point) =>
            {
                this.ShowStoryboard.Begin(); // Storyboard

                this.Vector = point;
                this.ThumbnailCanvasControl.Invalidate(); // Invalidate
                this.UpdateDragger(point);
                this.UpdateColor(point);
            };
            this.Operator.Single_Delta += (point) =>
            {
                this.Vector = point;
                this.ThumbnailCanvasControl.Invalidate(); // Invalidate
                this.UpdateDragger(point);
                this.UpdateColor(point);
            };
            this.Operator.Single_Complete += (point) =>
            {
                this.Vector = point;
                this.ThumbnailCanvasControl.Invalidate(); // Invalidate
                this.UpdateDragger(point);
                this.UpdateColor(point);

                this.HideStoryboard.Begin(); // Storyboard                                        
            };


            // Right
            this.Operator.Right_Start += (point) =>
            {
                this.Transformer.CacheMove(point);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point) =>
            {
                this.Transformer.Move(point);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point) =>
            {
                this.Transformer.Move(point);
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(center, space);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(center, space);

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(point, 1.05f);
                else
                    this.Transformer.ZoomOut(point, 1.05f);

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }


        private void ConstructScroller()
        {
            this.Thumb.DragStarted += async (s, e) =>
            {
                if (this.Target.Visibility == Visibility.Collapsed) return;
                await this.Scroller.RenderAsync(this.Target);
                this.Scroller.DragStarted(60, 120);
                this.Target.Visibility = Visibility.Collapsed;
            };
            this.Thumb.DragDelta += (s, e) => this.Scroller.DragDelta(e.HorizontalChange, e.VerticalChange);
            this.Thumb.DragCompleted += (s, e) => this.Scroller.DragCompleted();

            this.Scroller.DragPageDownCompleted += (s, e) => this.Target.Visibility = Visibility.Visible;
            this.Scroller.DragPageUpCompleted += (s, e) =>
            {
                this.Clear();
                this.Target.Visibility = Visibility.Visible;
            };
        }


        public void Clear()
        {
            this.Bitmap?.Dispose();
            this.Bitmap = null;
            this.CanWork = false;

            this.CanvasControl.Invalidate(); // Invalidate
        }

        public async void AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference == null) return;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    this.Bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                }
            }
            catch (Exception)
            {
            }

            if (this.Bitmap == null) return;
            this.CanWork = true;

            this.Transformer.BitmapSize = this.Bitmap.SizeInPixels;
            this.Transformer.Fit();

            this.CanvasControl.Invalidate(); // Invalidate

            this.AddStoryboard.Begin(); // Storyboard
        }

        private void UpdateDragger(Vector2 point)
        {
            Canvas.SetLeft(this.Dragger, point.X - 130 / 2);
            Canvas.SetTop(this.Dragger, point.Y - 130);
        }

        private void UpdateColor(Vector2 point)
        {
            Matrix3x2 matrix = this.Transformer.GetInverseMatrix();
            Vector2 inversePoint = Vector2.Transform(point, matrix);

            int x = (int)(inversePoint.X);
            int y = (int)(inversePoint.Y);

            int left = Math.Max(0, Math.Min(this.Transformer.Width - 1, x));
            int top = Math.Max(0, Math.Min(this.Transformer.Height - 1, y));

            this.Color = this.Bitmap.GetPixelColors(left, top, 1, 1).Single();
        }

    }
}