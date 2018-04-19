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
using 配色pro.Library;

namespace 配色pro.Control
{
    public sealed partial class GradientControl : UserControl
    {

        Color LeftColor = Color.FromArgb(255, 0, 121, 181);
        Color RightColor = Color.FromArgb(255, 255, 121, 181);
        Color GradientColor;

        public GradientControl()
        {
            this.InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Gradient();
        }


        #region Gradient：渐变

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Color GradientColor = Method.BlendColor(Slider.Value / 100, LeftColor, RightColor);
            App.Model.Color = Color.FromArgb(GradientColor.A, GradientColor.R, GradientColor.G, GradientColor.B);
        }





        private void LeftBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LeftColor = Color.FromArgb(App.Model.Color.A, App.Model.Color.R, App.Model.Color.G, App.Model.Color.B);
            Gradient();
        }
        private void RightBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RightColor = Color.FromArgb(App.Model.Color.A, App.Model.Color.R, App.Model.Color.G, App.Model.Color.B);
            Gradient();
        }


        #endregion

         
        private void Gradient()
        {
            Left.Color = LeftColor;
            Right.Color = RightColor;

            LeftBorder.Fill = new SolidColorBrush(LeftColor);
            RightBorder.Fill = new SolidColorBrush(RightColor);
          }
      }
}
