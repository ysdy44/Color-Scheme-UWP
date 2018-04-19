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

namespace 配色pro.Control
{
    public sealed partial class BlendControl : UserControl
    {
        Color BlendAColor = Color.FromArgb(255, 144, 202, 248);
        Color BlendBColor = Color.FromArgb(255, 64, 163, 244);
        Color BlendCColor = Color.FromArgb(255, 25, 114, 206);
        Color BlendDColor = Color.FromArgb(255, 8, 64, 149);

        Color BlendColor;

        double BlendAOpacity = 25;
        double BlendBOpacity = 50;
        double BlendCOpacity = 75;
        double BlendDOpacity = 100;

        public BlendControl()
        {
            this.InitializeComponent();
        }

        private void BlendGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //Slider
            BlendASlider.Value = BlendAOpacity;
            BlendBSlider.Value = BlendBOpacity;
            BlendCSlider.Value = BlendCOpacity;
            BlendDSlider.Value = BlendDOpacity;

            //Text
            BlendAText.Text = ((int)(BlendAOpacity)).ToString();
            BlendBText.Text = ((int)(BlendBOpacity)).ToString();
            BlendCText.Text = ((int)(BlendCOpacity)).ToString();
            BlendDText.Text = ((int)(BlendDOpacity)).ToString();

            //Opacity
            BlendARect.Opacity = BlendAOpacity / 100;
            BlendBRect.Opacity = BlendBOpacity / 100;
            BlendCRect.Opacity = BlendCOpacity / 100;
            BlendDRect.Opacity = BlendDOpacity / 100;

            //Rect
            BlendARect.Fill = new SolidColorBrush(BlendAColor);
            BlendBRect.Fill = new SolidColorBrush(BlendBColor);
            BlendCRect.Fill = new SolidColorBrush(BlendCColor);
            BlendDRect.Fill = new SolidColorBrush(BlendDColor);

            Blend();
        }


        #region Check：确认


        //Check
        private void BlendACheck_Loaded(object sender, RoutedEventArgs e)
        {
            BlendACheck.Checked += BlendACheck_Checked;
            BlendACheck.Unchecked += BlendACheck_Unchecked;
        }
        private void BlendACheck_Checked(object sender, RoutedEventArgs e)
        {
            BlendARect.Visibility = Visibility.Visible;
            BlendABack.Opacity = 1.0d;
            BlendASlider.IsEnabled = true;
            Blend();
        }
        private void BlendACheck_Unchecked(object sender, RoutedEventArgs e)
        {
            BlendARect.Visibility = Visibility.Collapsed;
            BlendABack.Opacity = 0.5d;
            BlendASlider.IsEnabled = false;
            Blend();
        }





        private void BlendBCheck_Loaded(object sender, RoutedEventArgs e)
        {
            BlendBCheck.Checked += BlendBCheck_Checked;
            BlendBCheck.Unchecked += BlendBCheck_Unchecked;
        }
        private void BlendBCheck_Checked(object sender, RoutedEventArgs e)
        {
            BlendBRect.Visibility = Visibility.Visible;
            BlendBBack.Opacity = 1.0d;
            BlendBSlider.IsEnabled = true;
            Blend();
        }
        private void BlendBCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            BlendBRect.Visibility = Visibility.Collapsed;
            BlendBBack.Opacity = 0.5d;
            BlendBSlider.IsEnabled = false;
            Blend();
        }





        private void BlendCCheck_Loaded(object sender, RoutedEventArgs e)
        {
            BlendCCheck.Checked += BlendCCheck_Checked;
            BlendCCheck.Unchecked += BlendCCheck_Unchecked;
        }
        private void BlendCCheck_Checked(object sender, RoutedEventArgs e)
        {
            BlendCRect.Visibility = Visibility.Visible;
            BlendCBack.Opacity = 1.0d;
            BlendCSlider.IsEnabled = true;
            Blend();
        }
        private void BlendCCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            BlendCRect.Visibility = Visibility.Collapsed;
            BlendCBack.Opacity = 0.5d;
            BlendCSlider.IsEnabled = false;
            Blend();
        }




        private void BlendDCheck_Loaded(object sender, RoutedEventArgs e)
        {
            BlendDCheck.Checked += BlendDCheck_Checked;
            BlendDCheck.Unchecked += BlendDCheck_Unchecked;
        }
        private void BlendDCheck_Checked(object sender, RoutedEventArgs e)
        {
            BlendDRect.Visibility = Visibility.Visible;
            BlendDBack.Opacity = 1.0d;
            BlendDSlider.IsEnabled = true;
            Blend();
        }
        private void BlendDCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            BlendDRect.Visibility = Visibility.Collapsed;
            BlendDBack.Opacity = 0.5d;
            BlendDSlider.IsEnabled = false;
            Blend();
        }


        #endregion


        #region Rect：赋色


        private void BlendARect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BlendAColor = App.Model.Color;
            BlendARect.Fill = new SolidColorBrush(App.Model.Color);
            Blend();
        }
        private void BlendBRect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BlendBColor = App.Model.Color;
            BlendBRect.Fill = new SolidColorBrush(App.Model.Color);
            Blend();
        }
        private void BlendCRect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BlendCColor = App.Model.Color;
            BlendCRect.Fill = new SolidColorBrush(App.Model.Color);
            Blend();
        }
        private void BlendDRect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BlendDColor = App.Model.Color;
            BlendDRect.Fill = new SolidColorBrush(App.Model.Color);
            Blend();
        }

        #endregion


        #region Slider：滑条

        private void BlendASlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BlendAOpacity = e.NewValue;
            BlendARect.Opacity = e.NewValue / 100;
            BlendAText.Text = ((int)(e.NewValue)).ToString();
            Blend();
        }
        private void BlendBSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BlendBOpacity = e.NewValue;
            BlendBRect.Opacity = e.NewValue / 100;
            BlendBText.Text = ((int)(e.NewValue)).ToString();
            Blend();
        }
        private void BlendCSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BlendCOpacity = e.NewValue;
            BlendCRect.Opacity = e.NewValue / 100;
            BlendCText.Text = ((int)(e.NewValue)).ToString();
            Blend();
        }
        private void BlendDSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BlendDOpacity = e.NewValue;
            BlendDRect.Opacity = e.NewValue / 100;
            BlendDText.Text = ((int)(e.NewValue)).ToString();

            Blend();
        }





        #endregion


        private void BlendRect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.Color = BlendColor;
        }




        private void Blend()
        {
            BlendColor = Colors.White;

            if (BlendDCheck.IsChecked == true)
                BlendColor = Method.ColorBlend(BlendDColor, BlendDOpacity, BlendColor, 100);

            if (BlendCCheck.IsChecked == true)
                BlendColor = Method.ColorBlend(BlendCColor, BlendCOpacity, BlendColor, 100);

            if (BlendBCheck.IsChecked == true)
                BlendColor = Method.ColorBlend(BlendBColor, BlendBOpacity, BlendColor, 100);

            if (BlendACheck.IsChecked == true)
                BlendColor = Method.ColorBlend(BlendAColor, BlendAOpacity, BlendColor, 100);

            BlendRect.Fill = new SolidColorBrush(BlendColor);
        }



    }
}
