using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using 配色pro.Library;

namespace 配色pro.Control
{
    public sealed partial class SpliteControl : UserControl
    {
        #region DependencyProperty：依赖属性

        public Visibility SpliteVisibility
        {
            set { SetValue(SpliteVisibilityProperty, value); }
        }
        public static readonly DependencyProperty SpliteVisibilityProperty =
            DependencyProperty.Register("SpliteVisibility", typeof(Visibility), typeof(SpliteControl), new PropertyMetadata(0, new PropertyChangedCallback(SpliteVisibilityOnChang)));
        private static void SpliteVisibilityOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SpliteControl Con = (SpliteControl)sender;

            if ((Visibility)e.NewValue == Visibility.Visible)
                Con.ShowMethod();
            else if ((Visibility)e.NewValue == Visibility.Collapsed)
                Con.FadeMethod();
        }

        #endregion 
        public SpliteControl()
        {
         this.InitializeComponent();
        }


        //侧栏按钮：打开侧栏
        private void SpliteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.SpliteVisibility = Visibility.Visible;
        }
        //侧栏面板：黑色遮罩
        private void Panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.SpliteVisibility = Visibility.Collapsed;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            App.Model.SpliteVisibility = Visibility.Collapsed;
        }



        #region Splite：侧栏


        //变换（实际变量）&记录X的临时变量
        //不要删掉后者，不然滑动时会有滞阻效果，也称进阻退留的楞次定律
        CompositeTransform CompositeTransform = new CompositeTransform();
        double CompositeTransformTranslateX;

        //左侧距离&左侧距离极限（向左拖动的目的地与最后的红线）
        //如果后者小于前者20，向左拖动可以越界20
        double LeftDistance = -300;
        double LeftDistanceLimbo = -300;

        //分界距离（处于左侧距离与右侧距离的中间）
        //拖动结束时判断其左倾还是右倾，决定打成左派还是右派
        double Boundary = -125;

        //右侧距离&右侧距离极限（向右拖动的目的地与最后的红线）
        //如果后者大于前者20，向右拖动可以越界20
        double RightDistance = 0;
        double RightDistanceLimbo = 0;


        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            Grid.RenderTransform = CompositeTransform;//绑定

            //通过侧栏面板的透明度获得现在的横位移
            CompositeTransformTranslateX = CompositeTransform.TranslateX = Panel.Opacity * (RightDistance - LeftDistance) + LeftDistance;

            //侧栏面板&侧栏阴影可视
            Panel.Visibility = Visibility.Visible;
            SpliteShadow.Opacity = 1.0d;
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //拖动（增量临时变量并赋值实际变量）
            CompositeTransformTranslateX += e.HorizontalChange;
            CompositeTransform.TranslateX = CompositeTransformTranslateX;

            //通过左右距离边界来判断是否越界
            if (CompositeTransform.TranslateX > RightDistanceLimbo)
                CompositeTransform.TranslateX = RightDistanceLimbo;
            else if (CompositeTransform.TranslateX < LeftDistanceLimbo)
                CompositeTransform.TranslateX = LeftDistanceLimbo;

            Panel.Opacity = (CompositeTransform.TranslateX - LeftDistance) / (RightDistance - LeftDistance);
        }
        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //通过分界距离判断侧栏是进还是出
            if (CompositeTransform.TranslateX >= Boundary)
            {
                App.Model.SpliteVisibility = Visibility.Visible;
                ShowMethod();
            }
            else if (CompositeTransform.TranslateX < Boundary)
            {
                App.Model.SpliteVisibility = Visibility.Collapsed;
                FadeMethod();
            }
        }

        #endregion


        //方法
        private void ShowMethod()
        {
            Panel.Visibility = Visibility.Visible;
            SpliteShadow.Opacity = 1.0d;

            SpliteShow.Begin();
        }
        private void FadeMethod()
        {
            SpliteFade.Begin();

            SpliteShadow.Opacity = 0.0d;
            Panel.Visibility = Visibility.Collapsed;
        }


        #region Share：分享


        DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();

        private void ShareButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Share：分享
            dataTransferManager.DataRequested += OnDataRequested;
            DataTransferManager.ShowShareUI();
        }

        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            //Share：分享
            DataRequest request = args.Request;
            request.Data.Properties.Title = "Share";
            request.Data.Properties.Description = "分享";
            request.Data.SetText(
                string.Format(
                        "ColorScheme pro\r\n" +
                 "——form Windows10 UWP Shop\r\n" +
                 "https://www.microsoft.com/store/productId/9NJXVM10VX16"
                    )
                );
        }




        #endregion

        private void OpensourceButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
         Launcher.LaunchUriAsync(new Uri("https://github.com/ysdy44/Color-Scheme-pro-UWP"));
         }


    }
}
