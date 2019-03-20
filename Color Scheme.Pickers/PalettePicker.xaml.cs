﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Color_Scheme.Pickers
{
    public class Square
    {
        public Vector2 Center = new Vector2(50, 50);
        public float Width = 100;
        public float Height = 100;
        public float HalfWidth => this.Width / 2;
        public float HalfHeight => this.Height / 2;
        public float StrokePadding = 12;
    }

    public partial class PalettePicker : UserControl
    {
        //Delegate
        public event ColorChangeHandler ColorChange = null;


        #region DependencyProperty


        private HSL hsl = new HSL { A = 255, H = 0, S = 1, L = 1 };
        private HSL _HSL
        {
            get => this.hsl;
            set
            {
                this.ColorChange?.Invoke(this, HSL.HSLtoRGB(value.A, value.H, value.S, value.L));

                this.hsl = value;
            }
        }
        public HSL HSL
        {
            get => this.hsl;
            set
            {
                this.Action(value);
                this.hsl = value;

                this.CanvasControl.Invalidate();
            }
        }


        #endregion



        bool IsPalette = false;
        Vector2 Vector;
        Action<HSL> Action;
        Square Square = new Square();


        public PalettePicker(PaletteBase paletteBase)
        {
            this.InitializeComponent();

            //Picker
            this.Slider.Minimum = paletteBase.Minimum;
            this.Slider.Maximum = paletteBase.Maximum;

            this.Slider.Value = paletteBase.GetValue(this.hsl);
            this.LinearGradientBrush.GradientStops = paletteBase.GetSliderBrush(this.hsl);

            this.Slider.ValueChangeDelta += (sender, value) => this.HSL = this._HSL = paletteBase.GetHSL(this.hsl, value);

            //Action
            this.Action = (HSL hsl) =>
            {
                this.Slider.Value = paletteBase.GetValue(hsl);
                this.LinearGradientBrush.GradientStops = paletteBase.GetSliderBrush(hsl);
            };

            //Canvas
            this.CanvasControl.SizeChanged += (sender, e) =>
            {
                this.Square.Center = e.NewSize.ToVector2() / 2;

                this.Square.Width = (float)e.NewSize.Width - this.Square.StrokePadding * 2;
                this.Square.Height = (float)e.NewSize.Height - this.Square.StrokePadding * 2;
            };
            this.CanvasControl.Draw += (sender, args) => paletteBase.Draw(this.CanvasControl, args.DrawingSession, this.hsl, this.Square.Center, this.Square.HalfWidth, this.Square.HalfHeight);



            //Manipulation
            this.CanvasControl.ManipulationMode = ManipulationModes.All;
            this.CanvasControl.ManipulationStarted += (sender, e) =>
            {
                this.Vector = e.Position.ToVector2() - this.Square.Center;

                this.IsPalette = Math.Abs(Vector.X) < this.Square.Width && Math.Abs(this.Vector.Y) < this.Square.Height;

                if (this.IsPalette) this.HSL = this._HSL = paletteBase.Delta(this.hsl, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationDelta += (sender, e) =>
            {
                this.Vector += e.Delta.Translation.ToVector2();

                if (this.IsPalette) this.HSL = this._HSL = paletteBase.Delta(this.hsl, this.Vector, this.Square.HalfWidth, this.Square.HalfHeight);
            };
            this.CanvasControl.ManipulationCompleted += (sender, e) => this.IsPalette = false;



            this.CanvasControl.Invalidate();
        }


    }



    /// <summary> Palette Base </summary>
    public abstract class PaletteBase
    {
        public string Name;
        public string Unit;
        public double Minimum;
        public double Maximum;

        public abstract HSL GetHSL(HSL HSL, double value);
        public abstract double GetValue(HSL HSL);

        public abstract GradientStopCollection GetSliderBrush(HSL HSL);

        public abstract void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSL HSL, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight);
        public abstract HSL Delta(HSL HSL, Vector2 v, float SquareHalfWidth, float SquareHalfHeight);
    }

    /// <summary> Palette Hue</summary>
    public class PaletteHue : PaletteBase
    {
        public PaletteHue()
        {
            this.Name = "Hue";
            this.Unit = "º";
            this.Minimum = 0;
            this.Maximum = 360;
        }

        public override HSL GetHSL(HSL HSL, double value) => new HSL(HSL.A, value, HSL.S, HSL.L);
        public override double GetValue(HSL HSL) => HSL.H;

        public override GradientStopCollection GetSliderBrush(HSL HSL)
        {
            byte A = HSL.A;
            double H = HSL.H;
            double S = HSL.S;
            double L = HSL.L;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0,
                    Color = HSL.HSLtoRGB(A, 0, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.16666667,
                    Color = HSL.HSLtoRGB(A, 60, S, L)
                },
                 new GradientStop()
                {
                    Offset = 0.33333333 ,
                    Color = HSL.HSLtoRGB(A, 120, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.5 ,
                    Color = HSL.HSLtoRGB(A, 180, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.66666667 ,
                    Color = HSL.HSLtoRGB(A, 240, S, L)
                },
                new GradientStop()
                {
                    Offset = 0.83333333 ,
                    Color = HSL.HSLtoRGB(A, 300, S, L)
                },
                new GradientStop()
                {
                    Offset = 1 ,
                    Color = HSL.HSLtoRGB(A, 0, S, L)
                },
            };
        }

        public override void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSL HSL, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight)
        {
            //Palette
            Rect rect = new Rect(Center.X - SquareHalfWidth, Center.Y - SquareHalfHeight, SquareHalfWidth * 2, SquareHalfHeight * 2);
            ds.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(CanvasControl, Windows.UI.Colors.White, HSL.HSLtoRGB(HSL.H)) { StartPoint = new Vector2(Center.X - SquareHalfWidth, Center.Y), EndPoint = new Vector2(Center.X + SquareHalfWidth, Center.Y) });
            ds.FillRoundedRectangle(rect, 4, 4, new CanvasLinearGradientBrush(CanvasControl, Windows.UI.Colors.Transparent, Windows.UI.Colors.Black) { StartPoint = new Vector2(Center.X, Center.Y - SquareHalfHeight), EndPoint = new Vector2(Center.X, Center.Y + SquareHalfHeight) });
            ds.DrawRoundedRectangle(rect, 4, 4, Windows.UI.Colors.Gray);

            //Thumb 
            float px = ((float)HSL.S - 50) * SquareHalfWidth / 50 + Center.X;
            float py = (50 - (float)HSL.L) * SquareHalfHeight / 50 + Center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        public override HSL Delta(HSL HSL, Vector2 v, float SquareHalfWidth, float SquareHalfHeight)
        {
            double S = 50 + v.X * 50 / SquareHalfWidth;
            double L = 50 - v.Y * 50 / SquareHalfHeight;

            return new HSL(HSL.A, HSL.H, S, L);
        }
    }

    /// <summary> Palette Saturation </summary>
    public class PaletteSaturation : PaletteBase
    {
        public CanvasGradientStop[] BackgroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color =  Windows.UI.Colors.Red },
            new CanvasGradientStop { Position = 0.16666667f, Color =Windows.UI.Colors.Yellow},
            new CanvasGradientStop { Position = 0.33333333f, Color = Color.FromArgb(255,0,255,0) },
            new CanvasGradientStop { Position = 0.5f, Color = Windows.UI.Colors.Cyan },
            new CanvasGradientStop { Position = 0.66666667f, Color = Windows.UI.Colors.Blue},
            new CanvasGradientStop { Position = 0.83333333f, Color =  Windows.UI.Colors.Magenta },
            new CanvasGradientStop { Position = 1.0f, Color =  Windows.UI.Colors.Red },
        };
        public CanvasGradientStop[] ForegroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color = Windows.UI.Colors.Transparent },
            new CanvasGradientStop { Position = 1.0f, Color = Windows.UI.Colors.Black }
        };

        public PaletteSaturation()
        {
            this.Name = "Saturation";
            this.Unit = "%";
            this.Minimum = 0;
            this.Maximum = 100;
        }

        public override HSL GetHSL(HSL HSL, double value) => new HSL(HSL.A, HSL.H, value, HSL.L);
        public override double GetValue(HSL HSL) => HSL.S;
        public override GradientStopCollection GetSliderBrush(HSL HSL)
        {
            byte A = HSL.A;
            double H = HSL.H;
            double S = HSL.S;
            double L = HSL.L;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0,
                    Color = HSL.HSLtoRGB(A, H, 0.0d, L)
                },
               new GradientStop()
                {
                    Offset = 1,
                    Color =HSL.HSLtoRGB(A, H, 100.0d, L)
                },
            };
        }

        public override void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSL HSL, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight)
        {
            //Palette
            Rect rect = new Rect(Center.X - SquareHalfWidth, Center.Y - SquareHalfHeight, SquareHalfWidth * 2, SquareHalfHeight * 2);
            using (CanvasLinearGradientBrush rainbow = new CanvasLinearGradientBrush(CanvasControl, this.BackgroundStops))
            {
                rainbow.StartPoint = new Vector2(Center.X - SquareHalfWidth, Center.Y);
                rainbow.EndPoint = new Vector2(Center.X + SquareHalfWidth, Center.Y);
                ds.FillRoundedRectangle(rect, 4, 4, rainbow);
            }
            using (CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(CanvasControl, this.ForegroundStops))
            {
                brush.StartPoint = new Vector2(Center.X, Center.Y - SquareHalfHeight);
                brush.EndPoint = new Vector2(Center.X, Center.Y + SquareHalfHeight);
                ds.FillRoundedRectangle(rect, 4, 4, brush);
            }
            ds.DrawRoundedRectangle(rect, 4, 4, Windows.UI.Colors.Gray);

            //Thumb 
            float px = ((float)HSL.H - 180) * SquareHalfWidth / 180 + Center.X;
            float py = ((float)(50 - HSL.L)) * SquareHalfHeight / 50 + Center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        public override HSL Delta(HSL HSL, Vector2 v, float SquareHalfWidth, float SquareHalfHeight)
        {
            double H = v.X * 180 / SquareHalfWidth + 180;
            double L = 50 - v.Y * 50 / SquareHalfHeight;
            return new HSL(HSL.A, H, HSL.S, L);
        }
    }

    /// <summary> Palette Lightness </summary>
    public class PaletteLightness : PaletteBase
    {
        public CanvasGradientStop[] BackgroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color =  Windows.UI.Colors.Red },
            new CanvasGradientStop { Position = 0.16666667f, Color =Windows.UI.Colors.Yellow},
            new CanvasGradientStop { Position = 0.33333333f, Color = Color.FromArgb(255,0,255,0) },
            new CanvasGradientStop { Position = 0.5f, Color = Windows.UI.Colors.Cyan },
            new CanvasGradientStop { Position = 0.66666667f, Color = Windows.UI.Colors.Blue},
            new CanvasGradientStop { Position = 0.83333333f, Color =  Windows.UI.Colors.Magenta },
            new CanvasGradientStop { Position = 1.0f, Color =  Windows.UI.Colors.Red },
        };
        public CanvasGradientStop[] ForegroundStops = new CanvasGradientStop[]
        {
            new CanvasGradientStop { Position = 0.0f, Color = Color.FromArgb(0,128,128,128) },
            new CanvasGradientStop { Position = 1.0f, Color = Windows.UI.Colors.White }
        };

        public PaletteLightness()
        {
            this.Name = "Lightness";
            this.Unit = "%";
            this.Minimum = 0;
            this.Maximum = 100;
        }

        public override HSL GetHSL(HSL HSL, double value) => new HSL(HSL.A, HSL.H, HSL.S, value);
        public override double GetValue(HSL HSL) => HSL.L;

        public override GradientStopCollection GetSliderBrush(HSL HSL)
        {
            byte A = HSL.A;
            double H = HSL.H;
            double S = HSL.S;
            double L = HSL.L;

            return new GradientStopCollection()
            {
                new GradientStop()
                {
                    Offset = 0.0f ,
                    Color = HSL.HSLtoRGB(A, H, S, 0)
                },
                new GradientStop()
                {
                    Offset = 1.0f,
                    Color = HSL.HSLtoRGB(A, H, S, 100)
                },
           };
        }

        public override void Draw(CanvasControl CanvasControl, CanvasDrawingSession ds, HSL HSL, Vector2 Center, float SquareHalfWidth, float SquareHalfHeight)
        {
            //Palette
            Rect rect = new Rect(Center.X - SquareHalfWidth, Center.Y - SquareHalfHeight, SquareHalfWidth * 2, SquareHalfHeight * 2);
            using (CanvasLinearGradientBrush rainbow = new CanvasLinearGradientBrush(CanvasControl, this.BackgroundStops))
            {
                rainbow.StartPoint = new Vector2(Center.X - SquareHalfWidth, Center.Y);
                rainbow.EndPoint = new Vector2(Center.X + SquareHalfWidth, Center.Y);
                ds.FillRoundedRectangle(rect, 4, 4, rainbow);
            }
            using (CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(CanvasControl, this.ForegroundStops))
            {
                brush.StartPoint = new Vector2(Center.X, Center.Y - SquareHalfHeight);
                brush.EndPoint = new Vector2(Center.X, Center.Y + SquareHalfHeight);
                ds.FillRoundedRectangle(rect, 4, 4, brush);
            }
            ds.DrawRoundedRectangle(rect, 4, 4, Windows.UI.Colors.Gray);

            //Thumb 
            float px = ((float)HSL.H - 180) * SquareHalfWidth / 180 + Center.X;
            float py = (50 - (float)HSL.S) * SquareHalfHeight / 50 + Center.Y;
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.Black, 5);
            ds.DrawCircle(px, py, 9, Windows.UI.Colors.White, 3);
        }
        public override HSL Delta(HSL HSL, Vector2 v, float SquareHalfWidth, float SquareHalfHeight)
        {
            double H = v.X * 180 / SquareHalfWidth + 180;
            double S = 50 - v.Y * 50 / SquareHalfHeight;
            return new HSL(HSL.A, H, S, HSL.L);
        }
    }

}
