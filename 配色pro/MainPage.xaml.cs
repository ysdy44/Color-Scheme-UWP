using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using 配色pro.Library;
using 配色pro.Model;

namespace 配色pro
{ 
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局


        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            JudgeWidth(e.NewSize.Width);
        }




        private void SpliteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.SpliteVisibility = Visibility.Visible; 
        }




        private async void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.Model.SelectedIndex == 0)
            {
                HomeText.Opacity = 1;
                ColorLine.Visibility = Visibility.Visible;
            }
            else
            {
                HomeText.Opacity = 0.4;
                ColorLine.Visibility = Visibility.Collapsed;
            }

            if (App.Model.SelectedIndex == 1)
            {
                EnumText.Opacity = 1;
                EnumLine.Visibility = Visibility.Visible;
            }
            else
            {
                EnumText.Opacity = 0.4;
                EnumLine.Visibility = Visibility.Collapsed;
            }

            if (App.Model.SelectedIndex == 2)
            {
                ImageText.Opacity = 1;
                ImageLine.Visibility = Visibility.Visible;
            }
            else
            {
                ImageText.Opacity = 0.4;
                ImageLine.Visibility = Visibility.Collapsed;
            }
        }

        private void HomeBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.SelectedIndex = 0;

            App.Model.Hsl = Method.RGBtoHSL(App.Model.Color);
        }
        private void EnumBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.SelectedIndex = 1;
        }
        private void ImageBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.SelectedIndex = 2;
        }


        #endregion

        #region Drop：拖拽


        private async void UserControl_Drop(object sender, DragEventArgs e)
        {
            //切换到画布枢轴面板
            App.Model.SelectedIndex = 2;


            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                items = items.OfType<StorageFile>().Where(s =>
                 s.FileType.ToUpper().Equals(".JPG") ||
                s.FileType.ToUpper().Equals(".JPEG") ||
                s.FileType.ToUpper().Equals(".PNG") ||
                s.FileType.ToUpper().Equals(".GIF") ||
                s.FileType.ToUpper().Equals(".BMP")
                ).ToList() as IReadOnlyList<IStorageItem>;

                if (items != null)//空或没有
                {
                    //切换到画布枢轴面板
                    App.Model.SelectedIndex = 2;

                    this.mainCanvasControl.ImageFile = items.First() as StorageFile;
                }
            }
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }



        //快捷键粘贴图片
        private  void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (App.Model.DialogVisibility == Visibility.Collapsed)
            {
                if (e.Key == VirtualKey.V)
                {
                    DataPackageView dataPackageView = Clipboard.GetContent();
                    this.mainCanvasControl.ImagePackage = dataPackageView;

                    e.Handled = true;
                }
            }
        }


        #endregion 



        #region Width：屏幕宽度


        //屏幕模式枚举
        private enum WidthEnum
        {
            Initialize,//初始状态

            PhoneNarrow,//手机竖屏
            PhoneStrath,//手机横屏

            Pad,//平板横屏
            Pc//电脑
        }
        WidthEnum owe = WidthEnum.Initialize;//OldWidthEnum：旧屏幕宽度枚举
        WidthEnum nwe = WidthEnum.Initialize;//NewWidthEnum：新屏幕宽度枚举


        private void JudgeWidth(double w)
        {
            //根据屏幕宽度判断
            if (w < 600) nwe = WidthEnum.PhoneNarrow;
            else if (w >= 600 && w < 800) nwe = WidthEnum.PhoneStrath;
            else if (w >= 800 && w < 1000) nwe = WidthEnum.Pad;
            else if (w >= 1000) nwe = WidthEnum.Pc;

            if (nwe != owe)//窗口变化过程中，新旧屏幕模式枚举不一样
            {
                //侧栏          
                if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath)//如果手机竖屏或手机横屏
                {
                    //顶栏
                    TittleText.Visibility = Visibility.Visible;
                    Grid.SetRow(PivotGrid, 1);
                    PivotGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
                else if (nwe == WidthEnum.Pad || nwe == WidthEnum.Pc)//如果平板或电脑
                {
                    //顶栏
                    TittleText.Visibility = Visibility.Collapsed;
                    Grid.SetRow(PivotGrid, 0);
                    PivotGrid.HorizontalAlignment = HorizontalAlignment.Center;
                }


                owe = nwe;
            }
        }

        #endregion


        #region ScrollViewer：滚动视图


        double TittleHeight = 44;


        private void HomeScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath)//如果手机竖屏或手机横屏
            {
                if (HomeScrollViewer.VerticalOffset > TittleHeight) TittleGrid.Height = 0;
                else TittleGrid.Height = TittleHeight - HomeScrollViewer.VerticalOffset;
            }
            else TittleGrid.Height = TittleHeight;
        }

        private void EnumScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath)//如果手机竖屏或手机横屏
            {
                if (EnumScrollViewer.VerticalOffset > TittleHeight) TittleGrid.Height = 0;
                else TittleGrid.Height = TittleHeight - EnumScrollViewer.VerticalOffset;
            }
            else TittleGrid.Height = TittleHeight;
        }

        private void MoneyScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

            if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath)//如果手机竖屏或手机横屏
            {
                if (MoneyScrollViewer.VerticalOffset > TittleHeight) TittleGrid.Height = 0;
                else TittleGrid.Height = TittleHeight - MoneyScrollViewer.VerticalOffset;
            }
            else TittleGrid.Height = TittleHeight;
        }


        #endregion



        private void DialogControl_Image()
        {
            mainCanvasControl.Image++;
        }
        
    }
}
