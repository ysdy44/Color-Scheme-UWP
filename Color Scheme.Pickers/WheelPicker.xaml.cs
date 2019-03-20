﻿using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Color_Scheme.Pickers
{
    public class Wheel
    {
        public static double VectorToH(Vector2 vector) => (((Math.Atan2(vector.Y, vector.X)) * 180.0 / Math.PI) + 360.0) % 360.0;
        public static float VectorToS(float vectorX, float squareRadio) => vectorX * 50 / squareRadio + 50;
        public static float VectorToL(float vectorY, float squareRadio) => 50 - vectorY * 50 / squareRadio;

        public static Vector2 HToVector(float h, float radio, Vector2 center) => new Vector2((float)Math.Cos(h) * radio + center.X, (float)Math.Sin(h) * radio + center.Y);
        public static float SToVector(double s, float squareRadio, float centerX) => ((float)s - 50) * squareRadio / 50 + centerX;
        public static float LToVector(double l, float squareRadio, float centerY) => (50 - (float)l) * squareRadio / 50 + centerY;
    }

    public sealed partial class WheelPicker : UserControl
    {

        //Delegate
        public event HSLChangeHandler HSLChange = null;


        #region DependencyProperty


        private HSL hsl = new HSL { A = 255, H = 0, S = 1, L = 1 };
        private HSL _HSL
        {
            get => this.hsl;
            set
            {
                this.HSLChange?.Invoke(this, value);

                this.hsl = value;
            }
        }
        public HSL HSL
        {
            get => this.hsl;
            set
            {
                byte A = value.A;
                double H = value.H;
                double S = value.S;
                double L = value.L;

                this.CanvasControl.Invalidate();

                this.hsl = value;
            }
        }



        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(SolidColorBrush), typeof(WheelPicker), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));


        #endregion


        Vector2 Center = new Vector2(50, 50);
        float Radio = 100;

        readonly float StrokeWidth = 8;
        float SquareRadio => (this.Radio - this.StrokeWidth) / 1.414213562373095f;

        Vector2 PaletteLeft => new Vector2(this.Center.X - this.SquareRadio, this.Center.Y);
        Vector2 PaletteRight => new Vector2(this.Center.X + this.SquareRadio, this.Center.Y);
        Vector2 PaletteTop => new Vector2(this.Center.X, this.Center.Y - this.SquareRadio);
        Vector2 PaletteBottom => new Vector2(this.Center.X, this.Center.Y + this.SquareRadio);


        public WheelPicker()
        {
            this.InitializeComponent();
        }


        private void CanvasControl_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            this.Center = e.NewSize.ToVector2() / 2;

            this.Radio = (float)Math.Min(e.NewSize.Width, e.NewSize.Height) / 2 - this.StrokeWidth;
        }
        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            //Wheel           
            args.DrawingSession.DrawCircle(this.Center, this.Radio, this.Stroke.Color, this.StrokeWidth * 2);
        
            float space = (float)(2 * Math.PI) / (int)(Math.PI * Radio * 2 / this.StrokeWidth);
            for (float angle = 0; angle < 6.2831853071795862f; angle += space)
            {
                Vector2 vector = Wheel.HToVector(angle, this.Radio, this.Center);
                Color color = HSL.HSLtoRGB(angle * 180.0 / Math.PI);
                args.DrawingSession.FillCircle(vector, this.StrokeWidth, color);
            }
            args.DrawingSession.DrawCircle(this.Center, this.Radio - this.StrokeWidth, this.Stroke.Color);
            args.DrawingSession.DrawCircle(this.Center, this.Radio + this.StrokeWidth, this.Stroke.Color);


            //Thumb
            Vector2 wheel = Wheel.HToVector((float)((this.HSL.H + 360.0) * Math.PI / 180.0), this.Radio, this.Center);
            args.DrawingSession.DrawCircle(wheel, 9, Windows.UI.Colors.Black, 5);
            args.DrawingSession.DrawCircle(wheel, 9, Windows.UI.Colors.White, 3);


            //Palette
            Rect rect = new Rect(this.Center.X - this.SquareRadio, this.Center.Y - this.SquareRadio, this.SquareRadio * 2, this.SquareRadio * 2);
            args.DrawingSession.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(this.CanvasControl, Windows.UI.Colors.White, HSL.HSLtoRGB(this.HSL.H))
            {
                StartPoint =this.PaletteLeft,
                EndPoint = this.PaletteRight
            });
            args.DrawingSession.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(this.CanvasControl, Windows.UI.Colors.Transparent, Windows.UI.Colors.Black)
            {
                StartPoint = this.PaletteTop,
                EndPoint = this.PaletteBottom
            });
            args.DrawingSession.DrawRoundedRectangle(rect, 4, 4, this.Stroke.Color);


            //Thumb 
            float paletteX = Wheel.SToVector(this.HSL.S, this.SquareRadio, this.Center.X);
            float paletteY = Wheel.LToVector(this.HSL.L, this.SquareRadio, this.Center.Y);
            args.DrawingSession.DrawCircle(paletteX, paletteY, 9, Windows.UI.Colors.Black, 5);
            args.DrawingSession.DrawCircle(paletteX, paletteY, 9, Windows.UI.Colors.White, 3);
        }



        bool IsWheel = false;
        bool IsPalette = false;
        Vector2 Vector;
        private void CanvasControl_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            this.Vector = e.Position.ToVector2() - this.Center;

            this.IsWheel = this.Vector.Length() + this.StrokeWidth > this.Radio && this.Vector.Length() - this.StrokeWidth < this.Radio;
            this.IsPalette = Math.Abs(this.Vector.X) < this.SquareRadio && Math.Abs(this.Vector.Y) < this.SquareRadio;

            if (this.IsWheel) this.HSL = this._HSL = new HSL(hsl.A, Wheel.VectorToH(this.Vector), hsl.S, hsl.L);
            if (this.IsPalette) this.HSL = this._HSL = new HSL(hsl.A, hsl.H, Wheel.VectorToS(this.Vector.X, this.SquareRadio), Wheel.VectorToL(this.Vector.Y, this.SquareRadio));
        }
        private void CanvasControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            this.Vector += e.Delta.Translation.ToVector2();

            if (this.IsWheel) this.HSL = this._HSL = new HSL(hsl.A, Wheel.VectorToH(this.Vector), hsl.S, hsl.L);
            if (this.IsPalette) this.HSL = this._HSL = new HSL(hsl.A, hsl.H, Wheel.VectorToS(this.Vector.X,this.SquareRadio), Wheel.VectorToL(this.Vector.Y, this.SquareRadio));
        }
        private void CanvasControl_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) => this.IsWheel = this.IsPalette = false;


    }
}
