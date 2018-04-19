using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class HSLControl : UserControl
    {

        #region DependencyProperty：依赖属性

        public HSL Hsl
        {
            get { return (HSL)GetValue(HslProperty); }
            set { SetValue(HslProperty, value); }
        }

        public static readonly DependencyProperty HslProperty =
            DependencyProperty.Register("Hsl", typeof(HSL), typeof(HSLControl), new PropertyMetadata(new HSL(), new PropertyChangedCallback(HslOnChang)));

        private static void HslOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            HSLControl Con = (HSLControl)sender;

            Con.Follow((HSL)e.NewValue);
        }

        #endregion

        public HSLControl()
        {
            this.InitializeComponent();
        }

        private void HSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Model.H = e.NewValue;
            Hi = (int)e.NewValue;

            FollowS(App.Model.Hsl);
            FollowL(App.Model.Hsl);
        }

        private void SSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Model.S = e.NewValue / 100;
            Si = (int)(e.NewValue);

            FollowH(App.Model.Hsl);
            FollowL(App.Model.Hsl);
        }
        private void LSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Model.L = e.NewValue / 100;
            Li = (int)(e.NewValue * 100);

            FollowH(App.Model.Hsl);
            FollowS(App.Model.Hsl);
        }






        //int变量
        int Hi;//（0~360）
        int Si;//（0~100）
        int Li;//（0~100）

        public void Follow(HSL hsl)
        {

            //H
            Hi = (int)hsl.H;

            HSlider.ValueChanged -= HSlider_ValueChanged;
            HSlider.Value = Hi;
            HSlider.ValueChanged += HSlider_ValueChanged;

            HText.Text = Hi.ToString() + "º";
            HA.Color = Method.HSLtoRGB(0, hsl.S, hsl.L);
            HB.Color = Method.HSLtoRGB(60, hsl.S, hsl.L);
            HC.Color = Method.HSLtoRGB(120, hsl.S, hsl.L);
            HD.Color = Method.HSLtoRGB(180, hsl.S, hsl.L);
            HE.Color = Method.HSLtoRGB(240, hsl.S, hsl.L);
            HF.Color = Method.HSLtoRGB(300, hsl.S, hsl.L);
            HG.Color = Method.HSLtoRGB(0, hsl.S, hsl.L);


            //S
            Si = (int)(hsl.S * 100);

            SSlider.ValueChanged -= SSlider_ValueChanged;
            SSlider.Value = Si;
            SSlider.ValueChanged += SSlider_ValueChanged;

            SText.Text = Si.ToString() + "%";
            SLeft.Color = Method.HSLtoRGB(hsl.H, 0.0d, hsl.L);
            SRight.Color = Method.HSLtoRGB(hsl.H, 1.0d, hsl.L);


            //L
            Li = (int)(hsl.L * 100);

            LSlider.ValueChanged -= LSlider_ValueChanged;
            LSlider.Value = Li;
            LSlider.ValueChanged += LSlider_ValueChanged;

            LText.Text = Li.ToString() + "%";
            LRight.Color = Method.HSLtoRGB(hsl.H, hsl.S, 1.0d);
        }

        public void FollowH(HSL hsl)
        {
            //H
            Hi = (int)hsl.H;

            HText.Text = Hi.ToString() + "º";
            HA.Color = Method.HSLtoRGB(0, hsl.S, hsl.L);
            HB.Color = Method.HSLtoRGB(60, hsl.S, hsl.L);
            HC.Color = Method.HSLtoRGB(120, hsl.S, hsl.L);
            HD.Color = Method.HSLtoRGB(180, hsl.S, hsl.L);
            HE.Color = Method.HSLtoRGB(240, hsl.S, hsl.L);
            HF.Color = Method.HSLtoRGB(300, hsl.S, hsl.L);
            HG.Color = Method.HSLtoRGB(0, hsl.S, hsl.L);
        }

        public void FollowS(HSL hsl)
        {
            //S
            Si = (int)(hsl.S * 100);

            SText.Text = Si.ToString() + "%";
            SLeft.Color = Method.HSLtoRGB(hsl.H, 0.0d, hsl.L);
            SRight.Color = Method.HSLtoRGB(hsl.H, 1.0d, hsl.L);
        }

        public void FollowL(HSL hsl)
        {
            //L
            Li = (int)(hsl.L * 100);

            LText.Text = Li.ToString() + "%";
            LRight.Color = Method.HSLtoRGB(hsl.H, hsl.S, 1.0d);
        }


    }
}
