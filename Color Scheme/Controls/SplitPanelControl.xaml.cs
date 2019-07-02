﻿using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Color_Scheme.Controls
{
    public sealed partial class SplitPanelControl : UserControl
    {
        public static readonly double OffsetX = 300;
        public static readonly TimeSpan ShowTime = new TimeSpan(0, 0, 0, 0, 500);
        public static readonly TimeSpan FadeTime = new TimeSpan(0, 0, 0, 0, 300);

        //Content
        public UIElement ContentChild { set => this.ContentBorder.Child = value; get => this.ContentBorder.Child; }

        //Opacity
        public double OpenOpacity
        {
            get => this.openOpacity;
            private set
            {
                this.DismissOverlayBackground.Visibility = (value == 0.0) ? Visibility.Collapsed : Visibility.Visible;
                this.DismissOverlayBackground.Opacity = value;
                this.SpliteLeftShadow.Opacity = value;

                this.openOpacity = value;
            }
        }
        private double openOpacity;

        public double TranslateX
        {
            set => this.translateX = value;
            private get
            {
                if (this.translateX > 0.0d) return 0.0d;
                else if (this.translateX < -this.ContentWidth) return -this.ContentWidth;
                else return this.translateX;
            }
        }
        private double translateX;


        //Open
        public bool IsOpen
        {
            get => this.isOpen;
            set
            {
                if (value) this.Open();
                else this.Close();

                this.isOpen = value;
            }
        }
        private bool isOpen;

        private void Open()
        {
            this.OpenOpacity = 1.0;
            this.ShowStoryboard.Begin();//Storyboard
        }
        private async void Close()
        {
            this.FadeStoryboard.Begin();//Storyboard
            await Task.Delay(SplitPanelControl.FadeTime);
            this.OpenOpacity = 0.0;
        }

        //Transform
        double ContentWidth;
        TranslateTransform TranslateTransform = new TranslateTransform();

        public SplitPanelControl()
        {
            this.InitializeComponent();
            this.ContentBorder.SizeChanged += (s, e) => this.ContentWidth = e.NewSize.Width;
            this.DismissOverlayBackground.Tapped += (s, e) => this.IsOpen = false;
            this.ContentBorder.Width = SplitPanelControl.OffsetX;
            this.TranslateX = -SplitPanelControl.OffsetX; ;

            //Storyboard
            this.ShowFrame1.To = 0;
            this.ShowFrame1.Duration = this.ShowFrame2.Duration = SplitPanelControl.ShowTime;

            this.FadeAnimation1.To = -SplitPanelControl.OffsetX; ;
            this.FadeAnimation1.Duration = this.FadeAnimation2.Duration = SplitPanelControl.FadeTime;
         
            //Transform
            this.TranslateTransform.X = -SplitPanelControl.OffsetX;
            this.PanelGrid.RenderTransform = this.TranslateTransform;

            //Manipulation
            this.PanelGrid.ManipulationMode = ManipulationModes.TranslateX;
            this.PanelGrid.ManipulationStarted += (s, e) => this.TranslateX = (this.OpenOpacity - 1) * this.ContentWidth;
            this.PanelGrid.ManipulationCompleted += (s, e) => this.IsOpen = (this.TranslateX > -this.ContentWidth / 2);
            this.PanelGrid.ManipulationDelta += (s, e) =>
            {
                //Opacity
                this.translateX += e.Delta.Translation.X;
                this.OpenOpacity = this.TranslateX / this.ContentWidth + 1;
                //Transform
                this.TranslateTransform.X = this.TranslateX;
                this.PanelGrid.RenderTransform = this.TranslateTransform;
            };
        }
    }
}
