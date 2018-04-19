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
using Windows.UI.Xaml.Navigation;


namespace 配色pro.Control
{
    public sealed partial class RGBControl : UserControl
    {

        #region DependencyProperty：依赖属性

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(RGBControl), new PropertyMetadata(Colors.Black, new PropertyChangedCallback(ColorOnChang)));

        private static void ColorOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RGBControl Con = (RGBControl)sender;

            Con.Follow((Color)e.NewValue);
        }

        #endregion

        public RGBControl()
        {
          this.InitializeComponent();
        }
        private void RSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Model.R = (byte)e.NewValue;
        }
        private void GSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Model.G = (byte)e.NewValue;
        }
        private void BSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Model.B = (byte)e.NewValue;
        }


        private void Follow(Color co)
        {
            //R       
            RSlider.ValueChanged -= RSlider_ValueChanged;
            RSlider.Value = co.R;
            RSlider.ValueChanged += RSlider_ValueChanged;

            RText.Text = co.R.ToString();
            RLeft.Color = Color.FromArgb(255, 0, co.G, co.B);
            RRight.Color = Color.FromArgb(255, 255, co.G, co.B);


            //G
            GSlider.ValueChanged -= GSlider_ValueChanged;
            GSlider.Value = co.G;
            GSlider.ValueChanged += GSlider_ValueChanged;

            GText.Text = co.G.ToString();
            GLeft.Color = Color.FromArgb(255, co.R, 0, co.B);
            GRight.Color = Color.FromArgb(255, co.R, 255, co.B);


            //B
            BSlider.ValueChanged -= BSlider_ValueChanged;
            BSlider.Value = co.B;
            BSlider.ValueChanged += BSlider_ValueChanged;

            BText.Text = co.B.ToString();
            BLeft.Color = Color.FromArgb(255, co.R, co.G, 0);
            BRight.Color = Color.FromArgb(255, co.R, co.G, 255);

        }


    }
}
