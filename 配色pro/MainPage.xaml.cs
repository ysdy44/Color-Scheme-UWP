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
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Newtonsoft.Json;
using 配色pro.Pickers;

namespace 配色pro
{

    public enum MainPageState
    {
        HomeType,
        EunmType,
        ImageType,
        PaletteType,
    }

    public sealed partial class MainPage : Page
    {
        private MainPageState state;
        public MainPageState State
        {
            get => this.state;
            set
            {
                this.HomeLabelControl.SeletedIndex =
                this.EunmLabelControl.SeletedIndex =
                this.ImageLabelControl.SeletedIndex = (int)value;

                this.HomePage.Visibility = (value == MainPageState.HomeType) ? Visibility.Visible : Visibility.Collapsed;
                this.EunmPage.Visibility = (value == MainPageState.EunmType) ? Visibility.Visible : Visibility.Collapsed;
                this.ImagePage.Visibility = (value == MainPageState.ImageType) ? Visibility.Visible : Visibility.Collapsed;

                this.state = value;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            this.ThemeButton.Tapped += (s, e) => this.ThemeControl.Theme = (this.ThemeControl.Theme == ElementTheme.Dark) ? ElementTheme.Light : ElementTheme.Dark;
            this.SplitButton.Tapped += (s, e) => this.SplitPanelControl.IsOpen = true;


            this.State = MainPageState.HomeType;
            this.HomeLabelControl.SeletedChanged += (value) => this.State = (MainPageState)value;
            this.EunmLabelControl.SeletedChanged += (value) => this.State = (MainPageState)value;
            this.ImageLabelControl.SeletedChanged += (value) => this.State = (MainPageState)value;
        }
    }
}



