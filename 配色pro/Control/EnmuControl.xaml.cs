using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class EnmuControl : UserControl
    {
        public EnmuControl()
        {
            this.InitializeComponent();
        }

        private void EnumGridView_Loaded(object sender, RoutedEventArgs e)
        {
            EnumGridView.ItemsSource = App.Model.EnmuList;
        }


        private void EnumGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ColorBrushCode code = e.ClickedItem as ColorBrushCode;

            if (code != null)
            {
                App.Model.Color = code.Color;
            }
        }

    }
}
