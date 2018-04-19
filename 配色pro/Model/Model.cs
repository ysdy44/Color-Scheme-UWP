using System.Numerics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Geometry;
using 配色pro.Library;

namespace 配色pro.Model
{
    public  class Model : INotifyPropertyChanged
    {
        public ObservableCollection<ColorBrushCode> EnmuList = new ObservableCollection<ColorBrushCode>();


        //主页索引
        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                this.OnPropertyChanged("SelectedIndex");
            }
        }





        //枚举索引
        private int enumIndex = 0;
        public int EnumIndex
        {
            get { return enumIndex; }
            set
            {
                enumIndex = value;
                this.OnPropertyChanged("EnumIndex");
            }
        } 
        
        
        
        //画布刷新（使用方法：Refresh++）
        private int refresh;
        public int Refresh
        {
            get { return refresh; }
            set
            {
                if (value > 1024) refresh = 0;
                else refresh = value;
                this.OnPropertyChanged("Refresh");
            }
        }


        #region Color：颜色


        private HSL hsl = new HSL
        {
            H = 201,
            S = 0.89,
            L = 0.37
        };
        public HSL Hsl
        {
            get { return hsl; }
            set
            {
                hsl = value;
                this.OnPropertyChanged("Hsl");
            }
        }
        public double H
        {
            get => hsl.H;
            set
            {
                hsl.H = value;
                Color = Method.HSLtoRGB(Hsl);
            }
        }
        public double S
        {
            get => hsl.S;
            set
            {
                hsl.S = value;
                Color = Method.HSLtoRGB(Hsl);
            }
        }
        public double L
        {
            get => hsl.L;

            set
            {
                hsl.L = value;
                Color = Method.HSLtoRGB(Hsl);
            }
        }



        //颜色
        private Color color =
            Color.FromArgb(255, 10, 121, 181);
        public Color Color
        {
            get { return color; }
            set
            {
                //笔刷颜色
                this.Brush.Color = color = value;

                //字体颜色
                this.Foreground = value.R + value.G + value.B < 600 ? White : Black;

                this.OnPropertyChanged("Color");
            }
        }

        public byte R
        {
            set
            {
                color.R = value;
                Hsl = Method.RGBtoHSL(color);
                Color = Color.FromArgb(color.A, color.R, color.G, color.B);
            }
        }
        public byte G
        {
            set
            {
                color.G = value;
                Hsl = Method.RGBtoHSL(color);
                Color = Color.FromArgb(color.A, color.R, color.G, color.B);
            }
        }
        public byte B
        {
            set
            {
                color.B = value;
                Hsl = Method.RGBtoHSL(color);
                Color = Color.FromArgb(color.A, color.R, color.G, color.B);
            }
        }

        //笔刷
        private SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 10, 121, 181));
        public SolidColorBrush Brush
        {
            get { return brush; }
            set
            {
                brush = value;
                this.OnPropertyChanged("Brush");
            }
        }

        //字体颜色
        private SolidColorBrush foreground = new SolidColorBrush(Colors.White);
        public SolidColorBrush Foreground
        {
            get { return foreground; }
            set
            {
                foreground = value;
                this.OnPropertyChanged("Foreground");
            }
        }

        private SolidColorBrush White = new SolidColorBrush(Colors.White);
        private SolidColorBrush Black = new SolidColorBrush(Colors.Black);


        #endregion


        #region  Concrol：控件宽高


        //控件宽高
        public int Width = 1024;
        public int Height = 1024;

        //画布位移
        private double x;
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                //  this.OnPropertyChanged("X");
            }
        }
        private double y;
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                //  this.OnPropertyChanged("Y");
            }
        }

        //画布宽高
        private double canvasWidth = 1024;
        public double CanvasWidth
        {
            get { return canvasWidth; }
            set
            {
                canvasWidth = value;
                this.OnPropertyChanged("CanvasWidth");
            }
        }
        private double canvasHeight = 1024;
        public double CanvasHeight
        {
            get { return canvasHeight; }
            set
            {
                canvasHeight = value;
                this.OnPropertyChanged("CanvasHeight");
            }
        }


        //画布比例
        public double XS
        {
            get => CanvasWidth / Width;
        }
        public double YS
        {
            get => CanvasHeight / Height;
        }
        public float SX
        {
            get => Width / (float)CanvasWidth;
        }
        public float SY
        {
            get => Height / (float)CanvasHeight;
        }


        //屏幕层与源图层
        public Point ScreenToCanvas(Point p)
        {
            return new Point((p.X - X) / XS, (p.Y - Y) / YS);
        }
        public Vector2 ScreenToCanvas(Vector2 p)
        {
            return new Vector2((float)((p.X - X) / XS), (float)((p.Y - Y) / YS));
        }
        public Rect ScreenToCanvas(Rect r)
        {
            return new Rect(
              (r.X - X) / XS,
               (r.Y - Y) / YS,
                r.Width / XS,
                r.Height / YS
                );
        }

        public Point CanvasToScreen(Point p)
        {
            return new Point(p.X * XS + X, p.Y * YS + Y);
        }
        public Vector2 CanvasToScreen(Vector2 p)
        {
            return new Vector2((float)(p.X * XS + X), (float)(p.Y * YS + Y));
        }
        public Rect CanvasToScreen(Rect r)
        {
            return new Rect(
                r.X * XS + X,
                r.Y * YS + Y,
                r.Width * XS,
                r.Height * YS
                );
        }

        //屏幕层与渲染层
        public Point ScreenToImage(Point p)
        {
            return new Point((p.X - X), (p.Y - Y));
        }
        public Point ImageToScreen(Point p)
        {
            return new Point(p.X + X, p.Y * +Y);
        }

        //对于画布的变换
        public Matrix3x2 Matrix
        {
            get => new Matrix3x2((float)XS, 0, 0, (float)YS, (float)X, (float)Y);
        }

        #endregion


        #region Binding：绑定


        //画布网格宽度
        public double GridWidth = 1024;
        public double GridHeight = 1024;

        //提示
        private string tip;
        public string Tip
        {
            get { return tip; }
            set
            {
                tip = value;
                this.OnPropertyChanged("Tip");
            }
        }

        private Visibility tipVisibility = Visibility.Collapsed;
        public Visibility TipVisibility
        {
            get { return tipVisibility; }
            set
            {
                tipVisibility = value;
                this.OnPropertyChanged("TipVisibility");
            }
        }


        //消息框
        private Visibility dialogVisibility = Visibility.Collapsed;
        public Visibility DialogVisibility
        {
            get { return dialogVisibility; }
            set
            {
                dialogVisibility = value;
                this.OnPropertyChanged("DialogVisibility");
            }
        }



        //侧栏
        private Visibility spliteVisibility = Visibility.Collapsed;
        public Visibility SpliteVisibility
        {
            get { return spliteVisibility; }
            set
            {
                spliteVisibility = value;
                this.OnPropertyChanged("SpliteVisibility");
            }
        }


        //Line：网格线
        private bool isline;
        public bool isLine
        {
            get { return isline; }
            set
            {
                isline = value;
                this.Refresh++;//画布刷新
                this.OnPropertyChanged("isLine");
            }
        }

        #endregion

        public Model() { }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
