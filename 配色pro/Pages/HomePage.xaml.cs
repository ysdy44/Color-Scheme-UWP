using 配色pro.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace 配色pro.Pages
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();

            this.WheelPicker.HSLChange += (s, value) =>
            {
                this.HSLPicker.HSL = value;
                this.RGBPicker.Color = HSL.HSLtoRGB(value);
            };
            this.RGBPicker.ColorChange += (s, value) =>
            {
                this.WheelPicker.HSL =
                this.HSLPicker.HSL = HSL.RGBtoHSL(value);
            };
            this.HSLPicker.HSLChange += (s, value) =>
            {
                this.WheelPicker.HSL = value;
                this.RGBPicker.Color = HSL.HSLtoRGB(value);
            };
        }
    }
}
