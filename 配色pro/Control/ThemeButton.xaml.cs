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
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;

namespace 配色pro.Control
{
    public sealed partial class ThemeButton : UserControl
    {
        //标题栏
        ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
        bool isLight//标题栏颜色
        {
            set
            {
                if (value == true)
                    TitleBar.ButtonInactiveBackgroundColor =
                        TitleBar.ButtonBackgroundColor =
                        TitleBar.InactiveBackgroundColor =
                        TitleBar.BackgroundColor = Color.FromArgb(255, 252, 252, 255);
                else
                    TitleBar.ButtonInactiveBackgroundColor =
                        TitleBar.ButtonBackgroundColor =
                        TitleBar.InactiveBackgroundColor =
                        TitleBar.BackgroundColor = Color.FromArgb(255, 18, 18, 21);
            }
        }

        //Delegate
        public delegate void ThemeChangeHandler(object sender, ElementTheme Theme);
        public event ThemeChangeHandler ThemeChange = null;

        public ThemeButton()
        {
           this.InitializeComponent();
        }

        #region Global：全局

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                ToNight.Begin();//Storyboard
                isLight = false;//标题栏颜色
            }
            else
            {
                ToLight.Begin();//Storyboard
                isLight = true;//标题栏颜色
            }
        }
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                if (frameworkElement.RequestedTheme == ElementTheme.Dark)
                {
                    frameworkElement.RequestedTheme = ElementTheme.Light;
                    ToLight.Begin();//Storyboard
                    isLight = true;//标题栏颜色
                    ThemeChange?.Invoke(this, ElementTheme.Light);//Delegate
                }
                else
                {
                    frameworkElement.RequestedTheme = ElementTheme.Dark;
                    ToNight.Begin();//Storyboard
                    isLight = false;//标题栏颜色
                    ThemeChange?.Invoke(this, ElementTheme.Dark);//Delegate
                }
            }
        }



        #endregion

    }
}
