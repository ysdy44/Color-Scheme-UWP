using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using 配色pro.Library;
using 配色pro.Model;

namespace 配色pro.Control
{
    public sealed partial class CopyControl : UserControl
    {
        //int变量
        int Hi;//（0~360）
        int Si;//（0~100）
        int Li;//（0~100）


        int 圆直径 = 180; //圆的直径
        int 圆边距 = 2; //圆环的边距
        int 矩边长 = 120; //矩形的宽高

        double 横位移;//起始位置
        double 纵位移;//起始位置
        double 横变化;//变化位置
        double 纵变化;//变化位置
        bool isThumb = false;//自身是否拖动 


        //Delegate
        public delegate void LayerHandler(bool isEnable);
        public event LayerHandler Layers;

        #region DependencyProperty：依赖属性

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(CopyControl), new PropertyMetadata(Colors.Black, new PropertyChangedCallback(ColorOnChang)));

        private static void ColorOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CopyControl Con = (CopyControl)sender;

            Con.Follow((Color)e.NewValue);
        }



        public HSL Hsl
        {
            get { return (HSL)GetValue(HslProperty); }
            set { SetValue(HslProperty, value); }
        }

        public static readonly DependencyProperty HslProperty =
            DependencyProperty.Register("Hsl", typeof(HSL), typeof(CopyControl), new PropertyMetadata(new HSL(), new PropertyChangedCallback(HslOnChang)));

        private static void HslOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CopyControl Con = (CopyControl)sender;

            Con.Follow((HSL)e.NewValue);
        }


        #endregion

        public CopyControl()
        {
            this.InitializeComponent();
        }

        #region Global：全局

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 600)
            {
                CopyRButton.Visibility = Visibility.Collapsed;
                CopyGButton.Visibility = Visibility.Collapsed;
                CopyBButton.Visibility = Visibility.Collapsed;
                CopyHButton.Visibility = Visibility.Collapsed;
                CopySButton.Visibility = Visibility.Collapsed;
                CopyLButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                CopyRButton.Visibility = Visibility.Visible;
                CopyGButton.Visibility = Visibility.Visible;
                CopyBButton.Visibility = Visibility.Visible;
                CopyHButton.Visibility = Visibility.Visible;
                CopySButton.Visibility = Visibility.Visible;
                CopyLButton.Visibility = Visibility.Visible;
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            赋形((int)e.NewSize.Width, (int)e.NewSize.Height);

            圆环赋色();
            矩形赋色();
        }

        #endregion


        private void 赋形(int 宽度, int 高度)
        {
            //《计算》
            //网格的宽和高，谁比较小谁就是圆的直径
            if (高度 <= 宽度) 圆直径 = 高度;
            else 圆直径 = 宽度;
            //圆环宽1/30
            圆边距 = 2;// 圆直径 / 30;
            //宽高为圆环的两倍差距
            矩边长 = (int)((圆直径 / 2 - 2 * 圆边距) * 1.414);

            //《赋形》
            //圆网格
            圆网格.Width = 圆网格.Height = 圆直径;
            //圆环
            圆.Width = 圆.Height = 圆直径;
            圆.StrokeThickness = 圆边距;
            EllipseTouch.Width = EllipseTouch.Height = 圆直径;

            //矩网格
            矩网格.Width = 矩网格.Height = 矩边长;
            //矩形
            矩.Width = 矩.Height = 矩边长;
            RectTouch.Width = RectTouch.Height = 矩边长;
        }


        #region Color：赋予颜色







        private void 圆环赋色()
        {
            Color[,] color = new Color[圆直径, 圆直径]; //数组格式为 [y, x]，与位图[x , y]相反

            for (int y = 0; y < 圆直径; y++)
            {
                for (int x = 0; x < 圆直径; x++)
                {
                    color[y, x] = Method.HSLtoRGB(((Math.Atan2(y - 圆直径 / 2, x - 圆直径 / 2) * 180.0 / Math.PI) + 450) % 360); //调用：获取颜色
                }
            }

            EllipseBackground.ImageSource = 配色pro.Library.Library.组转图(color); // 赋予笔刷
        }


        uint isDone = 0;
        Color[,] RectBitmapColors = new Color[256, 256]; //数组格式为 [y, x]，与位图[x , y]相反
        private void 矩形赋色()
        {
            if (isDone % 10 == 0)
            {
                double ss; //饱和度（1~2）
                double ll; //亮度（0~1）

                for (int y = 0; y < 255; y++)
                {
                    for (int x = 0; x < 255; x++)
                    {
                        ss = (double)x / 255d;
                        ll = (1.0d - (double)y / 255d);
                        RectBitmapColors[y, x] = Method.HSLtoRGB(App.Model.H, ss, ll); //调用：获取颜色          
                    }
                }
 
                RectBackground.ImageSource = 配色pro.Library.Library.组转图(RectBitmapColors); // 赋予笔刷
            }
            isDone++;
        }
        #endregion


        #region Move：移动


        private async void 圆拉扯(double x, double y) //根据角度改变位置方法
        {
            App.Model.H = ((Math.Atan2(y - 圆直径 / 2, x - 圆直径 / 2) * 180.0 / Math.PI) + 450) % 360; //偏转加取值
            App.Model.Hsl = new HSL
            {
                H = App.Model.H,
                S = App.Model.S,
                L = App.Model.L
            };

            矩形赋色();//一.《形形色色》：根据色相

            //计算X轴Y轴距离       
            x = (圆直径 - 圆边距) / 2 * Math.Sin(App.Model.H / 180 * Math.PI);
            y = (圆直径 - 圆边距) / 2 * -Math.Cos(App.Model.H / 180 * Math.PI);

            //改变位置
            Canvas.SetLeft(EllipseThumb, 圆直径 / 2 + x - EllipseThumb.Width / 2);
            Canvas.SetTop(EllipseThumb, 圆直径 / 2 + y - EllipseThumb.Height / 2);
        }


        bool isEllipseStart;
        private void EllipseTouch_DragStarted(object sender, DragStartedEventArgs e)
        {
            //判断
            isEllipseStart = true; // 开启《初始执行的事件》
            isThumb = true; //自身是否拖动
            Layers?.Invoke(false); //用户控件：禁用滚动

            //起始时，点击的位置就是圆点的位置
            圆拉扯(e.HorizontalOffset, e.VerticalOffset);

            //起始时，点击的位置就是横位移和纵位移的位置
            横位移 = e.HorizontalOffset;
            纵位移 = e.VerticalOffset;
        }
        private void EllipseTouch_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //《初始执行的事件》
            if (isEllipseStart)//从开始到变化的过程中执行一次
            {
                横变化 = 横位移; //把起始时的位置赋予到变化中，横变化（实时变化） +横位移（初始位置）才是真正的位置
                纵变化 = 纵位移;
            }
            isEllipseStart = false;//关掉《初始执行的事件》

            横变化 = 横变化 + e.HorizontalChange;
            纵变化 = 纵变化 + e.VerticalChange;

            圆拉扯(横变化, 纵变化);//二.《拉拉扯扯》
        }
        private void EllipseTouch_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false; //自身是否拖动           
            Layers?.Invoke(true); //用户控件：启用滚动
        }














        //《矩形》
        private void 矩拉扯(double 横, double 纵) //2.根据位置确定不超过边界
        {
            double x, y;

            //确定不超过横边界
            if (横 < 0) x = 0;
            else if (横 > 矩边长) x = 矩边长;
            else x = 横;

            //确定不超过纵边界
            if (纵 < 0) y = 0;
            else if (纵 > 矩边长) y = 矩边长;
            else y = 纵;

            Canvas.SetLeft(RectThumb, x - RectThumb.Width / 2);
            Canvas.SetTop(RectThumb, y - RectThumb.Height / 2);

            App.Model.S = (Canvas.GetLeft(RectThumb) + RectThumb.Width / 2) / 矩边长;
            App.Model.L = 1.0d - ((Canvas.GetTop(RectThumb) + RectThumb.Height / 2) / 矩边长);
            App.Model.Hsl = new HSL
            {
                H = App.Model.H,
                S = App.Model.S,
                L = App.Model.L
            };

            //HSL
            Si = (int)(App.Model.S * 100);
            Li = (int)(App.Model.L * 100);
        }

        bool isRectStart;
        private void RectTouch_DragStarted(object sender, DragStartedEventArgs e)
        {
            //判断
            isRectStart = true; // 开启《初始执行的事件》
            isThumb = true; //自身是否拖动
            Layers?.Invoke(false); //用户控件：禁用滚动

            //起始时，点击的位置就是圆点的位置
            矩拉扯(e.HorizontalOffset, e.VerticalOffset);//二.《拉拉扯扯》

            if (e.HorizontalOffset >= 0 && e.HorizontalOffset <= 矩边长 && e.VerticalOffset >= 0 && e.VerticalOffset <= 矩边长)
            {
                Canvas.SetLeft(RectThumb, e.HorizontalOffset - RectThumb.Width / 2);
                Canvas.SetTop(RectThumb, e.VerticalOffset - RectThumb.Height / 2);
            }

            //起始时，点击的位置就是横位移和纵位移的位置
            横位移 = e.HorizontalOffset;
            纵位移 = e.VerticalOffset;
        }
        private void RectTouch_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //《初始执行的事件》
            if (isRectStart)//从开始到变化的过程中执行一次
            {
                //把起始时的位置赋予到变化中，横变化（实时变化） +横位移（初始位置）才是真正的位置
                横变化 = 横位移;
                纵变化 = 纵位移;
            }
            isRectStart = false;//关掉《初始执行的事件》

            横变化 = 横变化 + e.HorizontalChange; //实时横变化
            纵变化 = 纵变化 + e.VerticalChange;//实时纵变化

            矩拉扯(横变化, 纵变化);//二.《拉拉扯扯》
        }
        private void RectTouch_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false; //自身是否拖动
            Layers?.Invoke(true); //用户控件：启用滚动
        }






        #endregion


        private void 色相变()//根据HSL改变位置（给依赖属性的在改变事件用的，需要判断）
        {
            if (isThumb == false)
            {
                矩形赋色();

                double x = 圆直径 / 2 + (圆直径 - 圆边距) / 2 * Math.Sin(App.Model.H / 180 * Math.PI) - EllipseThumb.Width / 2;
                double y = 圆直径 / 2 + (圆直径 - 圆边距) / 2 * -Math.Cos(App.Model.H / 180 * Math.PI) - EllipseThumb.Height / 2;
                Canvas.SetLeft(EllipseThumb, x);
                Canvas.SetTop(EllipseThumb, y);
            }
        }
        private void 饱和度变()//根据HSL改变位置（给依赖属性的在改变事件用的，需要判断）
        {
            if (isThumb == false) Canvas.SetLeft(RectThumb, App.Model.S * 矩边长 - RectThumb.Width / 2);
        }
        private void 亮度变()//根据HSL改变位置（给依赖属性的在改变事件用的，需要判断）
        {
            if (isThumb == false) Canvas.SetTop(RectThumb, (1.0d - App.Model.L) * 矩边长 - RectThumb.Height / 2);
        }



        #region Copy：复制参数




        //RGB
        private void CopyRNumberPicker_ValueChange(object sender, int value)
        {
            App.Model.R = (byte)value;
        }
        private void CopyGNumberPicker_ValueChange(object sender, int value)
        {
            App.Model.G = (byte)value;
        }
        private void CopyBNumberPicker_ValueChange(object sender, int value)
        {
            App.Model.B = (byte)value;
        }

        //HSl
        private void CopyHNumberPicker_ValueChange(object sender, int value)
        {
            App.Model.H = value;
         }
        private void CopySNumberPicker_ValueChange(object sender, int value)
        {
            App.Model.S = value / 100;
        }
        private void CopyLNumberPicker_ValueChange(object sender, int value)
        {
            App.Model.L = value / 100;
        }




        //RGB
        private void CopyRButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(App.Model.Color.R.ToString());

          App.Tip("Copyed");
        }
        private void CopyGButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(App.Model.Color.G.ToString());

          App.Tip("Copyed");
        }
        private void CopyBButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(App.Model.Color.B.ToString());

          App.Tip("Copyed");
        }

        //HSL
        private void CopyHButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(Hi.ToString());

            App.Tip("Copyed");
        }
        private void CopySButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(Si.ToString()); 

            App.Tip("Copyed");
        }
        private void CopyLButton_Tapped(object sender, TappedRoutedEventArgs e)
        { 
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(Li.ToString());

            App.Tip("Copyed");
        }







        #endregion


        private void Follow(Color co)
        {
            CopyRNumberPicker.Value = co.R;
            CopyGNumberPicker.Value = co.G;
            CopyBNumberPicker.Value = co.B;

            色相变();
            饱和度变();
            亮度变();
        }
        private void Follow(HSL hsl)
        {
            CopyHNumberPicker.Value = (int)hsl.H;
            CopySNumberPicker.Value = (int)(hsl.S * 100f);
            CopyLNumberPicker.Value = (int)(hsl.L * 100f);

            色相变();
            饱和度变();
            亮度变();
        }

    }
}
