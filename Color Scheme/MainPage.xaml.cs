using Color_Scheme.Controls;
using Color_Scheme.Model;
using Color_Scheme.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme
{
    public sealed partial class MainPage : Page
    {

        //@ViewModel
        private ObservableCollection<WidgetType> ItemsSource => App.ItemsSource;

        //@Converter
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseBooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;
        private Visibility ZeroToVisibilityConverter(int value) => value == 0 ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseZeroToVisibilityConverter(int value) => value == 0 ? Visibility.Collapsed : Visibility.Visible;
        private Visibility OneToVisibilityConverter(int value) => value == 1 ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseOneToVisibilityConverter(int value) => value == 1 ? Visibility.Collapsed : Visibility.Visible;
        private Visibility TwoToVisibilityConverter(int value) => value == 2 ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseTwoToVisibilityConverter(int value) => value == 2 ? Visibility.Collapsed : Visibility.Visible;

        public Color Color { get => this.Brush.Color; set => this.Brush.Color = value; }

        public MainPage()
        {
            this.InitializeComponent();
            this.ConstructFlowDirection();
            this.ConstructStrings();

            this.ConstructCanvasControl();
            this.ConstructCanvasOperator();


            // Drag and Drop 
            base.AllowDrop = true;
            base.Drop += async (s, e) =>
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                    foreach (IStorageItem item in items)
                    {
                        if (item is IStorageFile file)
                        {
                            this.SplitListView.SelectedIndex = 2;
                            this.AddAsync(file);
                            break;
                        }
                    }
                }
            };
            base.DragOver += (s, e) =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                //e.DragUIOverride.Caption = 
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsContentVisible = e.DragUIOverride.IsGlyphVisible = true;
            };

            // Index
            base.Loaded += (s, e) => this.SplitListView.SelectedIndex = 0;
            this.AButton.Click += (s, e) => this.SplitListView.SelectedIndex = 0;
            this.BButton.Click += (s, e) => this.SplitListView.SelectedIndex = 1;
            this.CButton.Click += (s, e) => this.SplitListView.SelectedIndex = 2;

            // ListView
            WidgetTypeContainer.ColorChanged += (s, value) => this.Color = value;
            this.SplitListView.SelectionChanged += (s, e) =>
            {
                switch (this.SplitListView.SelectedIndex)
                {
                    case 0:
                        WidgetTypeContainer.SetColor(this.Color);
                        break;
                }
            };

            // GridView
            this.GridView.Loaded += async (s, e) => this.GridView.ItemsSource = await XML.ConstructItemsFile();
            this.GridView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Item item)
                {
                    this.Color = item.Color;
                }
            };

            // CanvasControl
            this.CopyButton.Click += async (s, e) =>
            {
                if (this.Bitmap == null) return;
                await Clipboard2.CopyBitmapAsync(this.Bitmap.GetPixelBytes());
                this.CopyStoryboard.Begin(); // Storyboard
            };
            this.PasteButton.Click += async (s, e) =>
            {
                IRandomAccessStreamReference reference = await Clipboard2.PasteBitmapAsync();
                if (reference == null) return;
                this.AddAsync(reference);
                this.PasteStoryboard.Begin(); // Storyboard
            };
            this.AddButton.Click += async (s, e) => this.AddAsync(await FileUtil.PickSingleImageFileAsync(PickerLocationId.Desktop));
            this.AddButton2.Click += async (s, e) => this.AddAsync(await FileUtil.PickSingleImageFileAsync(PickerLocationId.Desktop));
            this.FloatActionButton.Click += async (s, e) =>
            {
                ContentDialogResult result = await this.ColorDialog.ShowAsync(this.Color);

                if (this.ColorDialog.GetColor() is Color color)
                {
                    this.Color = color;
                    switch (this.SplitListView.SelectedIndex)
                    {
                        case 0:
                            WidgetTypeContainer.SetColor(color);
                            break;
                    }
                }
            };

            // Split
            this.SplitListView.ItemClick += (s, e) => this.SplitView.IsPaneOpen = false;
            this.PCSplitButton.Click += (s, e) => this.SplitView.IsPaneOpen = false;
            this.PhoneSplitButton.Click += (s, e) => this.SplitView.IsPaneOpen = true;

            // Dialog
            this.AboutButton.Click += async (s, e) => await this.AboutDialog.ShowInstance();
            this.SettingButton.Click += async (s, e) => await this.SettingDialog.ShowInstance();
            this.SettingItem.Tapped += async (s, e) =>
            {
                this.SplitView.IsPaneOpen = false;
                await this.SettingDialog.ShowInstance();
            };

            // Setting
            this.SettingDialog.LanguageSetted += (s, language) =>
            {
                this.ConstructFlowDirection();
                this.ConstructStrings();
            };
        }

    }
}