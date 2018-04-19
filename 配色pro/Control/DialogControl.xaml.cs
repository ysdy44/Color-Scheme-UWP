using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using 配色pro.Model;


namespace 配色pro.Control
{
    public sealed partial class DialogControl : UserControl
    {

        #region DependencyProperty：依赖属性

        public Visibility DialogVisibility
        {
            set { SetValue(DialogVisibilityProperty, value); }
        }
        public static readonly DependencyProperty DialogVisibilityProperty =
            DependencyProperty.Register("DialogVisibility", typeof(Visibility), typeof(DialogControl), new PropertyMetadata(0, new PropertyChangedCallback(DialogVisibilityOnChang)));
        private static void DialogVisibilityOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DialogControl Con = (DialogControl)sender;

            if ((Visibility)e.NewValue == Visibility.Visible)
                Con.ShowMethod();
            else if ((Visibility)e.NewValue == Visibility.Collapsed)
                Con.FadeMethod();
        }



        public int SelectedIndex
        {
            set { SetValue(SelectedIndexProperty, value); }
        }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(DialogControl), new PropertyMetadata(0, new PropertyChangedCallback(SelectedIndexOnChang)));
        private static void SelectedIndexOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DialogControl Con = (DialogControl)sender;

            if ((int)e.NewValue == 1)
            {
                Con.AddShadowPanel.Visibility = Visibility.Visible;
                Con.DeleteShadowPanel.Visibility = Visibility.Visible;
                Con.EnmuShow.Begin();
            }
            else
            {
                Con.AddShadowPanel.Visibility = Visibility.Collapsed;
                Con.DeleteShadowPanel.Visibility = Visibility.Collapsed;
            }

            if ((int)e.NewValue == 2)
            {
                Con.ImageShadowPanel.Visibility = Visibility.Visible;
                Con.LineShadowPanel.Visibility = Visibility.Visible;
                Con.ImageShow.Begin();
            }
            else
            {
                Con.ImageShadowPanel.Visibility = Visibility.Collapsed;
                Con.LineShadowPanel.Visibility = Visibility.Collapsed;
            }
        }



        #endregion 

        public DialogControl()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }

        #region Dialog：消息框


        private void Ellipse_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            FadeEllipse.Begin();
        }
        private void Ellipse_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ShowEllipse.Begin();
            App.Model.DialogVisibility = Visibility.Visible;

            Textbox.Text = ColorToString(App.Model.Color);
        }
        private void Panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.DialogVisibility = Visibility.Collapsed;
        }






        //文本按钮
        private void CopyRButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
             dataPackage.SetText(ColorToString(App.Model.Color));

            App.Tip("Copyed");
        }
        private void OKButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                App.Model.Color = StringToColor(Textbox.Text);
            }
            catch (Exception)
            { }

            App.Model.DialogVisibility = Visibility.Collapsed;
        }
        private void CancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.DialogVisibility = Visibility.Collapsed;
        }



        #endregion


        //颜色于字符串转换
        private Color StringToColor(string s)
        {
            //转化为八进制颜色字符串（原理我也不知道，网上抄的）
            int rgb = int.Parse(s, System.Globalization.NumberStyles.HexNumber);
            byte r = (byte)((rgb >> 16) & 0xff);
            byte g = (byte)((rgb >> 8) & 0xff);
            byte b = (byte)((rgb >> 0) & 0xff);

            return Color.FromArgb(255, r, g, b);
        }
        private string ColorToString(Color c)
        {
            return c.R.ToString("x2") + c.G.ToString("x2") + c.B.ToString("x2").ToString();
        }







        //方法
        private void ShowMethod()
        {
            Panel.Visibility = Visibility.Visible;
            Grid.Visibility = Visibility.Visible;

            DialogShow.Begin();
        }
        private void FadeMethod()
        {
            DialogFade.Begin();

            Panel.Visibility = Visibility.Collapsed;
            Grid.Visibility = Visibility.Collapsed;
        }




        #region Enum：枚举


        private void AddButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ColorBrushCode brush = new ColorBrushCode
            {
                Name = "Untitled",
                Color = App.Model.Color
            };
            App.Model.EnmuList.Insert(0, brush);
        }


        private void DeleteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int indedx = App.Model.EnumIndex;

            if (indedx >= 0 && indedx < App.Model.EnmuList.Count)
            {
                App.Model.EnmuList.RemoveAt(indedx);
            }
        }


        #endregion


        #region Image：图片

        //Delegate
        public delegate void ImageHandler();
        public event ImageHandler Image;

        private void ImageButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image?.Invoke(); //用户控件：Value改变
        }

        private void LineButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.Model.isLine==true)
            {
                App.Model.isLine = false;
            }
            else
            {
                App.Model.isLine = true;
             }
        }

        #endregion

    }
}
